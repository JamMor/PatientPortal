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
        [HttpGet("{inbox}")]
        public IActionResult Inbox(string inbox)
        {
            MessagingLink userLink = _context.MessagingLinks
                .TagWith("MessageLinkQuery")
                .FirstOrDefault(link => link.MessagingLinkId == linkId);

            int unreadTotalCount = userLink.UnreadMessages?.Count() ?? 0;
            ViewBag.unreadTotalCount = unreadTotalCount;

            if(userLink.PatientId != null)
            {
                int unreadPatientCount = unreadTotalCount;
            }
            else if(userLink.StaffId != null)
            {
                int unreadPatientCount = userLink.UnreadMessages?.Where(unread => unread.WithPatient == true)
                    .Count() ?? 0;
                    
                ViewBag.unreadPatientCount = unreadPatientCount;
                ViewBag.unreadStaffCount = unreadTotalCount - unreadPatientCount;
            }

            var messageQuery = _context.Conversations
                    .Where(convo => convo.ConversationParticipants
                        .Any(joined => joined.MessagingLinkId == linkId));

            //Patient Inbox - If "patient" specified, or null (or any incorrect route) default here
            if(inbox != "staff")
            {

                List<Conversation> conversations = messageQuery
                    .Where(convo => convo.WithPatient == true)
                    .ToList();

                ViewBag.inbox = "patient";

                return View("Inbox", conversations);
            }
            
            //Staff Inbox
            else
            {
                List<Conversation> conversations = messageQuery
                    .Where(convo => convo.WithPatient == false)
                    .ToList();

                ViewBag.inbox = "staff";

                return View("Inbox", conversations);
            }
        }

        [HttpGet("new")]
        public IActionResult NewMessageForm()
        {
            List<Recipient> otherStaff = _context.Staff
                .Where(staff => staff.StaffId != linkId)
                .Select(staff => new Recipient()
                {
                    LinkId = staff.MessagingLink.MessagingLinkId,
                    Name = staff.FullName(),
                    Role = staff.Role,
                })
                .ToList();

            NewMessageFormView newMessageFormViewModel = new NewMessageFormView()
            {
                Recipients = otherStaff
            };

            return View("NewMessage", newMessageFormViewModel);
        }
    }
}