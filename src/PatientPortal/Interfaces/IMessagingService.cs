using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessagingService : IDisposable
    {
        //COMMANDS
        
        void CreateConversation(int linkId, NewConversationFormView newConversationFormView);
        void CreateReply(int linkId, int conversationId, ReplyView newReply);
        bool MarkRead(int linkId, int messageId);

        //QUERIES

        MessagingLink GetMessagingLink(int linkId);
        int GetUnreadTotalCount(MessagingLink messagingLink);
        int GetUnreadPatientCount(MessagingLink messagingLink);
        Recipient GetPatientRecipient(int? toLinkId);
        List<Recipient> GetAllOtherStaffAsRecipients(int linkId, int? toLinkId);
        IQueryable<Conversation> GetAllConversationsForInbox(int linkId, ConversationSearch inboxFilters);

    }
}