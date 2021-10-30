using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public MessagingController(PatientPortalContext context)
        {
            _context = context;
        }

        //===========================Inbox Manager==============================
        [HttpGet("{inbox?}")]
        public IActionResult Inbox(string inbox)
        {
            MessagingLink userLink = _context.MessagingLinks
                .TagWith("MessageLinkQuery")
                .Include(link => link.UnreadMessages)
                .FirstOrDefault(link => link.MessagingLinkId == linkId);

            //Unread Messages Count
            int unreadTotalCount = userLink.UnreadMessages?.Count() ?? 0;
            ViewBag.unreadTotalCount = unreadTotalCount;

            if(userLink.PatientId != null)
            {
                if(inbox != "")
                {
                    RedirectToAction("Inbox", new {inbox = ""});
                }
                int unreadPatientCount = unreadTotalCount;
            }
            else if(userLink.StaffId != null)
            {
                if(inbox != "staff" || inbox != "patient")
                {
                    RedirectToAction("Inbox", new {inbox = "patient"});
                }

                //Staff and Patient Specific Unread Counts
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
        public IActionResult NewConversationForm()
        {
            List<Recipient> otherStaff = _context.Staff
                .Where(staff => staff.MessagingLink.MessagingLinkId != linkId)
                .Select(staff => new Recipient()
                {
                    LinkId = staff.MessagingLink.MessagingLinkId,
                    Name = staff.FullName(),
                    Role = staff.Role,
                })
                .ToList();

            NewConversationFormView newConversationFormViewModel = new NewConversationFormView()
            {
                Recipients = otherStaff
            };

            return View("NewMessage", newConversationFormViewModel);
        }
        
        [HttpPost("new")]
        public IActionResult NewConversation(NewConversationFormView newConversationFormView)
        {
            List<int> recipientIds = newConversationFormView.Recipients
                .Where(recipient => recipient.Selected == true)
                .Select(recipient => recipient.LinkId)
                .ToList();

            List<ConversationParticipant> conversationParticipants = recipientIds
                .Select(id => new ConversationParticipant() {MessagingLinkId = id})
                .ToList();
            conversationParticipants.Add(new ConversationParticipant() {MessagingLinkId = (int)linkId});

            List<Unread> unreadFor = recipientIds
                .Select(id => new Unread() {MessagingLinkId = id, WithPatient = newConversationFormView.WithPatient})
                .ToList();

            // List<MessagingLink> recipients = _context.MessagingLinks
            //     .Where(link => recipientIds.Contains(link.MessagingLinkId))
            //     .ToList();
            
            Conversation newConversation = new Conversation()
            {
                Subject = newConversationFormView.Subject,
                WithPatient = newConversationFormView.WithPatient,
                Messages = new List<Message>
                {
                    new Message()
                    {
                        MessageText = newConversationFormView.MessageText,
                        MessagingLinkId = (int)linkId,
                        UnreadBy = unreadFor
                    }
                },
                ConversationParticipants = conversationParticipants
            };

            _context.Conversations.Add(newConversation);
            
            _context.SaveChanges();
            return RedirectToAction("Inbox");
        }
    }
}