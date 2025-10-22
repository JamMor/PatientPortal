using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Authorization;
using PatientPortal.DTOs;
using PatientPortal.Extensions;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Route("/provider/inbox")]
    public class MessagingController : Controller
    {
        private int? linkId => User.GetMessageLinkId();

        private IMessagingService _messagingService;
        private IMessagingViewService _messagingViewService;
        private readonly IAuthorizationService _authorizationService;

        public MessagingController(
            IMessagingService messagingService,
            IMessagingViewService messagingViewService,
            IAuthorizationService authorizationService
        )
        {
            _messagingService = messagingService;
            _messagingViewService = messagingViewService;
            _authorizationService = authorizationService;
        }

        //===========================Inbox Manager==============================
        [HttpGet("{inbox?}")]
        public async Task<IActionResult> Inbox(string inbox, InboxSearchForm searchForm)
        {
            // Check if user can manage patients (staff member)
            bool isUserStaff = User.GetStaffId().HasValue;

            if (!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            //Redirect to appropriate URL's for patient or staff member

            // Patient Users
            // TODO: Revisit patient inbox logic when patients get login access
            // Currently patients don't have login, so this branch is not used
            if (!isUserStaff)
            {
                // Force patient inbox
                if (inbox != "")
                {
                    return RedirectToAction("Inbox", new { inbox = "" });
                }
            }
            // Staff Users
            else
            {
                var messagePatientsAuthorization = await _authorizationService.AuthorizeAsync(User, PolicyNames.MessagePatients);
                bool CanMessagePatients = messagePatientsAuthorization.Succeeded;

                if (!CanMessagePatients)
                {
                    inbox = InboxType.Staff.Route;
                }

                // If invalid inbox type, redirect to default staff inbox (Patient Inbox)
                if (InboxType.FromRoute(inbox) == null)
                {
                    return RedirectToAction("Inbox", new { inbox = InboxType.Patient.Route });
                }
            }

            var query = new InboxQuery
            {
                Type = InboxType.FromRoute(inbox) ?? InboxType.Patient,
                OnlyUnread = searchForm.OnlyUnread,
                Paging = new Paginator
                {
                    ResultsPerPage = searchForm.ResultsPerPage,
                    CurrentPage = searchForm.CurrentPage,
                },
            };

            MessageInboxView? inboxView = _messagingViewService.ReturnInboxView((int)linkId, query);

            if (inboxView == null)
            {
                return NotFound();
            }

            return View("Inbox", inboxView);
        }

        [HttpGet("new")]
        public IActionResult NewConversationForm(int? toLinkId)
        {
            if (!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            NewConversationFormView newConversationFormViewModel =
                _messagingViewService.NewConversationForm((int)linkId, toLinkId);

            return View("NewMessageForm", newConversationFormViewModel);
        }

        [HttpPost("new")]
        public IActionResult NewConversation(NewConversationFormInput newConversationFormInput)
        {
            if (!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var conversationDTO = newConversationFormInput.ToConversationDTO();
                    var firstMessageDTO = newConversationFormInput.ToMessageDTO();
                    
                    _messagingService.CreateConversation((int)linkId, conversationDTO, firstMessageDTO);

                    string inboxRoute = newConversationFormInput.WithPatient
                        ? InboxType.Patient.Route
                        : InboxType.Staff.Route;
                    return RedirectToAction("Inbox", new { inbox = inboxRoute });
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the conversation.");
                }
            }

            var form = _messagingViewService.NewConversationForm((int)linkId, null)
                .ApplyInput(newConversationFormInput);
            return View("NewMessageForm", form);
        }

        [HttpPost("reply/{conversationId}/new")]
        public IActionResult NewReply(int conversationId, ReplyView newReply)
        {
            if (!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            string returnRoute = InboxType.FromRoute(newReply.ReturnRoute)?.Route ?? InboxType.Patient.Route;

            if (ModelState.IsValid)
            {
                try
                {
                    var newReplyDTO = newReply.ToMessageDTO();

                    _messagingService.CreateReply((int)linkId, conversationId, newReplyDTO);
                }
                catch
                {
                    // Log the exception (not implemented here)
                }
            }

            return RedirectToAction("Inbox", new { inbox = returnRoute });
        }

        [HttpPost("message/read")]
        public IActionResult MarkRead(int messageId)
        {
            if (!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            bool markedRead = _messagingService.MarkRead((int)linkId, messageId);

            if (markedRead)
            {
                return Ok(new { MessageId = messageId, MarkedUnread = true });
            }

            return NoContent();
        }
    }
}
