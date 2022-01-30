using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Route("/provider/inbox")]
    public class MessagingController : Controller
    {
        private int? linkId
        {
            get
            {
                return HttpContext.Session.GetInt32("MessageLinkId");
            }
        }
        private int? uuid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }
        private bool IsLoggedIn
        {
            get
            {
                return uuid != null;
            }
        }

        private PatientPortalContext _context;
        private IMessagingService _messagingService;
        public MessagingController(PatientPortalContext context, IMessagingService messagingService)
        {
            _context = context;
            _messagingService = messagingService;
        }

        //===========================Inbox Manager==============================
        [HttpGet("{inbox?}")]
        public IActionResult Inbox(string inbox)
        {
            //
            MessageInboxView inboxView = new MessageInboxView();
            //
            
            MessagingLink userLink = _messagingService.GetMessagingLink((int)linkId);

            //Unread Messages Count
            inboxView.UnreadTotal = _messagingService.GetUnreadTotalCount(userLink);

            //Redirect to appropriate URL's for patient or staff member and 
            //separate inbox counts for staff
            if(userLink.UserType == "Patient")
            {
                if(inbox != "")
                {
                    RedirectToAction("Inbox", new {inbox = ""});
                }
                inboxView.UnreadPatient = inboxView.UnreadTotal;
            }
            else if(userLink.UserType == "Staff")
            {
                if(inbox != "staff" || inbox != "patient")
                {
                    RedirectToAction("Inbox", new {inbox = "patient"});
                }

                //Staff and Patient Specific Unread Counts
                    
                inboxView.UnreadPatient = _messagingService.GetUnreadPatientCount(userLink);
                inboxView.UnreadStaff = inboxView.UnreadTotal - inboxView.UnreadPatient;
            }

            
            bool isPatientInbox = inbox != "staff";
            
            inboxView.Conversations = _messagingService.ConversationQuery((int)linkId, isPatientInbox);
            
            inboxView.InboxType = isPatientInbox ? "patient" : "staff";

            return View("Inbox", inboxView);
        }

        [HttpGet("new")]
        public IActionResult NewConversationForm(int? toLinkId)
        {
            
            NewConversationFormView newConversationFormViewModel = _messagingService.NewConversationForm((int)linkId, toLinkId);

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