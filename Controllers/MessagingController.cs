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
            inboxView.UnreadTotal = userLink.UnreadMessages?.Count() ?? 0;

            //Redirect to appropriate URL's for patient or staff member and 
            //separate inbox counts for staff
            if(userLink.PatientId != null)
            {
                if(inbox != "")
                {
                    RedirectToAction("Inbox", new {inbox = ""});
                }
                inboxView.UnreadPatient = inboxView.UnreadTotal;
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
                    
                inboxView.UnreadPatient = unreadPatientCount;
                inboxView.UnreadStaff = inboxView.UnreadTotal - inboxView.UnreadPatient;
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

                inboxView.InboxType = "patient";

            }
            
            //Staff Inbox
            else
            {
                messageQuery = messageQuery
                    .Where(convo => convo.WithPatient == false);
                
                inboxView.InboxType = "staff";

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
                                Name = p.MessagingLink.UserType == "Patient" ? 
                                    p.MessagingLink.Patient.FullName() : p.MessagingLink.Staff.FullName(),
                                Role = p.MessagingLink.UserType == "Patient" ? 
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
        public IActionResult NewConversationForm(int? toLinkId)
        {
            
            List<Recipient> otherStaff = _context.Staff
                .Where(staff => staff.MessagingLink.MessagingLinkId != linkId)
                .OrderBy(staff => staff.Role)
                .ThenBy(staff => staff.LastName)
                .Select(staff => new Recipient()
                {
                    LinkId = staff.MessagingLink.MessagingLinkId,
                    Name = staff.FullName(),
                    Role = staff.Role,
                    Selected = staff.MessagingLink.MessagingLinkId == toLinkId
                })
                .ToList();

            NewConversationFormView newConversationFormViewModel = new NewConversationFormView()
            {
                Recipients = otherStaff
            };

            //If linked from patient info, add patient to patient recipient.
            if(toLinkId != null)
            {
                // // For some reason this will return a messageLink that does not have the patient included
                // MessagingLink addressedLink = _context.MessagingLinks
                //     .Where(m => m.MessagingLinkId == toLinkId)
                //     .Include(m => m.Patient)
                //     .FirstOrDefault();
                
                Recipient patientRecipient = _context.MessagingLinks
                    .Include(m => m.Patient)
                    .Where(m => m.MessagingLinkId == toLinkId && m.PatientId != null)
                    .Select(m => new Recipient()
                    {
                        LinkId = m.MessagingLinkId,
                        Name = m.Patient.FullName(),
                        Role = "Patient",
                        Selected = true
                    })
                    .FirstOrDefault();

                if(patientRecipient != null)
                {
                    newConversationFormViewModel.PatientRecipient = patientRecipient;
                    newConversationFormViewModel.WithPatient = true;
                };
            };

            return View("NewMessage", newConversationFormViewModel);
        }
        
        [HttpPost("new")]
        public IActionResult NewConversation(NewConversationFormView newConversationFormView)
        {
            //Gets Id's of everyone to receive message
            List<int> recipientIds = newConversationFormView.Recipients
                .Where(recipient => recipient.Selected == true)
                .Select(recipient => recipient.LinkId)
                .ToList();

            //Adds PatientId if present
            if(newConversationFormView.PatientRecipient != null)
            {
                recipientIds.Add(newConversationFormView.PatientRecipient.LinkId);
            }


            //Adds everyone to conversation including current user (sender)
            List<ConversationParticipant> conversationParticipants = recipientIds
                .Select(id => new ConversationParticipant() {MessagingLinkId = id})
                .ToList();
            conversationParticipants.Add(new ConversationParticipant() {MessagingLinkId = (int)linkId});

            //Sets first message as unread for all other recipients
            List<Unread> unreadFor = recipientIds
                .Select(id => new Unread() {MessagingLinkId = id, WithPatient = newConversationFormView.WithPatient})
                .ToList();
            
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

                thisConversation.UpdatedAt = newMessage.CreatedAt;

                _context.SaveChanges();

                return RedirectToAction("Inbox");
            }

            return View("Inbox");
        }

        // public IActionResult MarkRead()
        // {

        // }
    }
}