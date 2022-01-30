using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessagingService : IDisposable
    {
        //COMMANDS

        // Message GetMessagebyId(int messageId);
        // List<Message> GetMessagebyQuery(MessageSearch searchQuery);
        // void CreateConversation(NewConversationFormView newConversationFormView);
        // void CreateMessage(int conversationId, ReplyView messageInfo);
        // void DeleteMessage(int messageId);
        
        NewConversationFormView NewConversationForm(int linkId, int? toLinkId);
        void CreateConversation(int linkId, NewConversationFormView newConversationFormView);
        void CreateReply(int linkId, int conversationId, ReplyView newReply);
        bool MarkRead(int linkId, int messageId);

        //QUERIES

        MessagingLink GetMessagingLink(int linkId);
        int GetUnreadTotalCount(MessagingLink messagingLink);
        int GetUnreadPatientCount(MessagingLink messagingLink);
        List<InboxConversation> ConversationQuery(int linkId, bool isPatientInbox);

    }
}