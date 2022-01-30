using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class MessagingViewService : IMessagingViewService
    {
        private PatientPortalContext _context;
        private IMessagingService _messagingService;
        public MessagingViewService(PatientPortalContext context, IMessagingService messagingService)
        {
            _context = context;
            _messagingService = messagingService;
        }

        //COMMANDS

        // public NewConversationFormView NewConversationForm(int linkId, int? toLinkId)
        // {
        //     List<Recipient> otherStaff = _context.Staff
        //         .Include(staff => staff.MessagingLink)
        //         .Where(staff => staff.MessagingLink.MessagingLinkId != linkId)
        //         .OrderBy(staff => staff.Role)
        //         .ThenBy(staff => staff.LastName)
        //         .Select(staff => new Recipient()
        //         {
        //             LinkId = staff.MessagingLink.MessagingLinkId,
        //             Name = staff.FullName(),
        //             Role = staff.Role,
        //             Selected = staff.MessagingLink.MessagingLinkId == toLinkId
        //         })
        //         .ToList();

        //     NewConversationFormView newConversationFormViewModel = new NewConversationFormView()
        //     {
        //         Recipients = otherStaff
        //     };

        //     //If linked from patient info, add patient to patient recipient.
        //     if(toLinkId != null)
        //     {
        //         // // For some reason this will return a messageLink that does not have the patient included
        //         // MessagingLink addressedLink = _context.MessagingLinks
        //         //     .TagWith("Unused Link Query that wouldn't return patient")
        //         //     .Include(m => m.Patient)
        //         //     .Where(m => m.MessagingLinkId == toLinkId)
        //         //     .FirstOrDefault();
                
        //         Recipient patientRecipient = _context.MessagingLinks
        //             .TagWith("Current Link Query")
        //             .Include(m => m.Patient)
        //             .Where(m => m.MessagingLinkId == toLinkId && m.PatientId != null)
        //             .Select(m => new Recipient()
        //             {
        //                 LinkId = m.MessagingLinkId,
        //                 Name = m.Patient.FullName(),
        //                 Role = "Patient",
        //                 Selected = true
        //             })
        //             .FirstOrDefault();

        //         if(patientRecipient != null)
        //         {
        //             newConversationFormViewModel.PatientRecipient = patientRecipient;
        //             newConversationFormViewModel.WithPatient = true;
        //         };
        //     };
        //     return newConversationFormViewModel;
        // }

        public MessageInboxView ReturnInboxView(int linkId, bool isPatientInbox)
        {
            MessagingLink messageLink = _messagingService.GetMessagingLink(linkId);
            int unreadTotal = _messagingService.GetUnreadTotalCount(messageLink);
            int unreadPatient = _messagingService.GetUnreadPatientCount(messageLink);

            MessageInboxView inboxView = new MessageInboxView()
            {
                UnreadTotal = unreadTotal,
                UnreadPatient = unreadPatient,
                UnreadStaff = unreadTotal -unreadPatient,
                IsPatientInbox = isPatientInbox,
                Conversations = _messagingService.ConversationQuery(linkId, isPatientInbox)
            };

            return inboxView;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PatientService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}