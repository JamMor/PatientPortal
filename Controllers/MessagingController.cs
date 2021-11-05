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
            //
            MessageInboxView inboxView = new MessageInboxView();
            //
            //Get MessagingLink for user
            MessagingLink userLink = _context.MessagingLinks
                .TagWith("MessageLinkQuery")
                .Include(link => link.UnreadMessages)
                .FirstOrDefault(link => link.MessagingLinkId == linkId);

            //Unread Messages Count
            int unreadTotalCount = userLink.UnreadMessages?.Count() ?? 0;
            ViewBag.unreadTotalCount = unreadTotalCount;

            //Redirect to appropriate URL's for patient or staff member and 
            //separate inbox counts for staff
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
                int unreadPatientCount = userLink.UnreadMessages
                    ?.Where(unread => unread.WithPatient == true)
                    .Count() ?? 0;
                    
                ViewBag.unreadPatientCount = unreadPatientCount;
                ViewBag.unreadStaffCount = unreadTotalCount - unreadPatientCount;
            }

            var messageQuery = _context.Conversations
                    .Include(convo => convo.Messages)
                        .ThenInclude(msg => msg.UnreadBy)
                    .Include(convo => convo.ConversationParticipants)
                        .ThenInclude(partic => partic.MessagingLink)
                        .ThenInclude(link => link.Patient)
                    .Include(convo => convo.ConversationParticipants)
                        .ThenInclude(partic => partic.MessagingLink)
                        .ThenInclude(link => link.Staff)
                    .Where(convo => convo.ConversationParticipants
                        .Any(joined => joined.MessagingLinkId == linkId));

            //Patient Inbox - If "patient" specified, or null default here
            if(inbox != "staff")
            {
                messageQuery = messageQuery
                    .Where(convo => convo.WithPatient == true);

                ViewBag.inbox = "patient";

            }
            
            //Staff Inbox
            else
            {
                messageQuery = messageQuery
                    .Where(convo => convo.WithPatient == false);
                
                ViewBag.inbox = "staff";

            }

            List<InboxConversation> conversations = messageQuery
                    .Select( c => new InboxConversation()
                    {
                        ConversationId = c.ConversationId,
                        Subject = c.Subject,
                        Participating = c.ConversationParticipants
                            .Select(p => new InboxRecipient()
                            {
                                LinkId = p.MessagingLinkId,
                                Name = p.MessagingLink.UserType() == "Patient" ? 
                                    p.MessagingLink.Patient.FullName() : p.MessagingLink.Staff.FullName(),
                                Role = p.MessagingLink.UserType() == "Patient" ? 
                                    "Patient" : p.MessagingLink.Staff.Role
                            })
                            .ToList(),
                        Messages = c.Messages
                            .Select(m => new InboxMessage()
                            {
                                MessageId = m.MessageId,
                                SenderId = m.MessagingLinkId,
                                MessageText = m.MessageText,
                                Sent = m.CreatedAt,
                                Unread = m.UnreadBy
                                    .Any(u => u.MessagingLinkId == (int)linkId)
                            })
                            .OrderBy(m => m.Sent)
                            .ToList(),
                        DateCreated = c.CreatedAt,
                        DateLastMessage = c.UpdatedAt
                    })
                    .OrderByDescending(c => c.DateLastMessage)
                    .ToList();

            inboxView.Conversations = conversations;

            return View("Inbox", inboxView);
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

        [HttpPost("reply/{conversationId}/new")]
        public IActionResult NewReply(int conversationId, ReplyView newReply)
        {
            if(ModelState.IsValid)
            {
                Conversation thisConversation = _context.Conversations
                    .Include(c => c.ConversationParticipants)
                    .SingleOrDefault(c => c.ConversationId == conversationId);

                List<Unread> unreadFor = thisConversation.ConversationParticipants
                    .Select(p => new Unread()
                    {
                        MessagingLinkId = p.MessagingLinkId,
                        WithPatient = thisConversation.WithPatient
                    })
                    .Where(p => p.MessagingLinkId != (int)linkId)
                    .ToList();

                Message newMessage = new Message()
                {
                    MessagingLinkId = (int)linkId,
                    ConversationId = conversationId,
                    MessageText = newReply.MessageText,
                    UnreadBy = unreadFor
                };

                _context.Messages.Add(newMessage);
                _context.SaveChanges();

                return RedirectToAction("Inbox");
            }

            return View("Inbox");
        }
    }
}