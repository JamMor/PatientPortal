using System.Linq;
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
            IAuthorizationService authorizationService)
        {
            _messagingService = messagingService;
            _messagingViewService = messagingViewService;
            _authorizationService = authorizationService;
        }

        private class InboxType
        {
            public const string WithoutPatients = "staff";
            public const string WithPatients = "patient";
        }

        //===========================Inbox Manager==============================
        [HttpGet("{inbox?}")]
        public async Task<IActionResult> Inbox(string inbox, ConversationSearch inboxFilters, Paginator paginationSettings)
        {
            paginationSettings.ResultsPerPage = 5;
            
            // Check if user can manage patients (staff member)
            bool isUserStaff = User.GetStaffId().HasValue;

            if(!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }
            
            
            //Redirect to appropriate URL's for patient or staff member

            // Patient Users
            // TODO: Revisit patient inbox logic when patients get login access
            // Currently patients don't have login, so this branch is not used
            if(!isUserStaff)
            {
                // Force patient inbox
                if(inbox != "")
                {
                    return RedirectToAction("Inbox", new {inbox = ""});
                }
            }
            // Staff Users
            else
            {
                var messagePatientsAuthorization = await _authorizationService.AuthorizeAsync(User, PolicyNames.MessagePatients);
                bool CanMessagePatients = messagePatientsAuthorization.Succeeded;

                if (!CanMessagePatients)
                {
                    inbox = InboxType.WithoutPatients;
                }

                // If invalid inbox type, redirect to default staff inbox                
                if(inbox != InboxType.WithoutPatients && inbox != InboxType.WithPatients)
                {
                    return RedirectToAction("Inbox", new {inbox = InboxType.WithPatients});
                }
            }

            inboxFilters.IsPatientInbox = inbox != InboxType.WithoutPatients;
            MessageInboxView? inboxView = _messagingViewService.ReturnInboxView((int)linkId, inboxFilters, paginationSettings);

            if(inboxView == null)
            {
                return NotFound();
            }

            return View("Inbox", inboxView);
        }

        [HttpGet("new")]
        public IActionResult NewConversationForm(int? toLinkId)
        {
            if(!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }
            
            NewConversationFormView newConversationFormViewModel = _messagingViewService.NewConversationForm((int)linkId, toLinkId);

            return View("NewMessageForm", newConversationFormViewModel);
        }
        
        [HttpPost("new")]
        public IActionResult NewConversation(NewConversationFormInput newConversationFormInput)
        {
            if(!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            if(ModelState.IsValid)
            {
                try
                {
                    var conversationDTO = newConversationFormInput.ToConversationDTO();
                    var firstMessageDTO = newConversationFormInput.ToMessageDTO();
                    
                    _messagingService.CreateConversation((int)linkId, conversationDTO, firstMessageDTO);
                    
                    return RedirectToAction("Inbox");
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the conversation.");
                }
            }

            var form = _messagingViewService.NewConversationForm((int)linkId, null).ApplyInput(newConversationFormInput);
            return View("NewMessageForm", form);

        }

        [HttpPost("reply/{conversationId}/new")]
        public IActionResult NewReply(int conversationId, ReplyView newReply)
        {
            if(!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            if(ModelState.IsValid)
            {
                try
                {
                    var newReplyDTO = newReply.ToMessageDTO();
                    
                    _messagingService.CreateReply((int)linkId, conversationId, newReplyDTO);
                    
                    return RedirectToAction("Inbox");
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the reply.");
                }

            }

            return View("Inbox", newReply);
        }

        [HttpPost("message/read")]
        public IActionResult MarkRead(int messageId)
        {
            if(!linkId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve Messaging information. Please ensure you are logged in and try again.");
            }

            bool markedRead = _messagingService.MarkRead((int)linkId, messageId);

            if(markedRead)
            {
                return Ok(new {MessageId = messageId, MarkedUnread = true});
            }

            return NoContent();
        }
    }
}