using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Route("/provider/messages")]
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
        public MessagingController(PatientPortalContext context)
        {
            _context = context;
        }

        //===========================Inbox Manager==============================
        [HttpGet("staff")]
        public IActionResult StaffInbox()
        {
            MessagingLink userLink = _context.MessagingLinks
                .TagWith("MessageLinkQuery")
                .FirstOrDefault(link => link.MessagingLinkId == linkId);

            int countAllMessages = userLink.UnreadMessages.Count;
            int countPatientMessages = userLink.UnreadMessages
                .Where(unread => unread.WithPatient == true)
                .Count();
            int countStaffMessages = countAllMessages - countPatientMessages;

            List<Conversation> staffMessages = _context.Conversations
                .Where(convo => convo.ConversationParticipants.Any(partic => partic.MessagingLinkId == linkId)
                    && convo.WithPatient == false)
                .ToList();

            return View("Inbox", staffMessages);
        }
        
        [HttpGet("patient")]
        public IActionResult PatientInbox()
        {
            MessagingLink userLink = _context.MessagingLinks
                .TagWith("MessageLinkQuery")
                .FirstOrDefault(link => link.MessagingLinkId == linkId);

            int countAllMessages = userLink.UnreadMessages.Count;
            int countPatientMessages = userLink.UnreadMessages
                .Where(unread => unread.WithPatient == true)
                .Count();
            int countStaffMessages = countAllMessages - countPatientMessages;

            List<Conversation> patientMessages = _context.Conversations
                .Where(convo => convo.ConversationParticipants.Any(partic => partic.MessagingLinkId == linkId)
                    && convo.WithPatient == true)
                .ToList();

            return View("Inbox", patientMessages);
        }

        // [HttpGet("patient")]
        // public IActionResult PatientMessages()
        // {
        // }
        
        // [HttpGet("")]
        // public IActionResult StaffMessages()
        // {
        //     MessagingLink linkData = _context.MessagingLinks
        //         .Include(link => link.ParticipatingConversations)
        //         .ThenInclude(joinedconvos => joinedconvos.Conversation)
        //         .ThenInclude(convos => convos.Messages)
        //         .Include(link => link.UnreadMessages)
        //         .FirstOrDefault(link => link.MessagingLinkId == linkId);

        //     return View("Inbox", linkData);
        // }
        
    }
}