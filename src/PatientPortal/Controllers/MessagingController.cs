using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Authorization;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

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
            
            bool isUserStaff = User.GetStaffId().HasValue;
            // Check if user can manage patients (staff member)
            
            
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
            MessageInboxView inboxView = _messagingViewService.ReturnInboxView((int)linkId, inboxFilters, paginationSettings);

            return View("Inbox", inboxView);
        }

        [HttpGet("new")]
        public IActionResult NewConversationForm(int? toLinkId)
        {
            
            NewConversationFormView newConversationFormViewModel = _messagingViewService.NewConversationForm((int)linkId, toLinkId);

            return View("NewMessageForm", newConversationFormViewModel);
        }
        
        [HttpPost("new")]
        public IActionResult NewConversation(NewConversationFormView newConversationFormView)
        {
            if(ModelState.IsValid)
            {
                _messagingService.CreateConversation((int)linkId, newConversationFormView);
                return RedirectToAction("Inbox");
            }

            return View("NewMessageForm", newConversationFormView);

        }

        [HttpPost("reply/{conversationId}/new")]
        public IActionResult NewReply(int conversationId, ReplyView newReply)
        {
            if(ModelState.IsValid)
            {
                _messagingService.CreateReply((int)linkId, conversationId, newReply);

                return RedirectToAction("Inbox");
            }

            return View("Inbox");
        }

        [HttpPost("message/read")]
        public IActionResult MarkRead(int messageId)
        {
            bool markedRead = _messagingService.MarkRead((int)linkId, messageId);

            if(markedRead)
            {
                return Ok(new {MessageId = messageId, MarkedUnread = true});
            }

            return NoContent();
        }
    }
}