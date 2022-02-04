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

        private IMessagingService _messagingService;
        private IMessagingViewService _messagingViewService;
        public MessagingController(IMessagingService messagingService, IMessagingViewService messagingViewService)
        {
            _messagingService = messagingService;
            _messagingViewService = messagingViewService;
        }

        //===========================Inbox Manager==============================
        [HttpGet("{inbox?}")]
        public IActionResult Inbox(string inbox)
        {
            bool isUserPatient = HttpContext.Session.GetString("Role") == "Patient";
            
            //Redirect to appropriate URL's for patient or staff member
            if(isUserPatient)
            {
                if(inbox != "")
                {
                    RedirectToAction("Inbox", new {inbox = ""});
                }
            }
            else if(!isUserPatient)
            {
                if(inbox != "staff" || inbox != "patient")
                {
                    RedirectToAction("Inbox", new {inbox = "patient"});
                }
            }

            bool isPatientInbox = inbox != "staff";
            MessageInboxView inboxView = _messagingViewService.ReturnInboxView((int)linkId, isPatientInbox);

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