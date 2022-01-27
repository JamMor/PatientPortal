using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessageService : IDisposable
    {

        Message GetMessagebyId(int messageId);
        // List<Message> GetMessagebyQuery(MessageSearch searchQuery);
        void CreateConversation(NewConversationFormView newConversationFormView);
        void CreateMessage(int conversationId, ReplyView messageInfo);
        void DeleteMessage(int messageId);
        
        void MarkRead(int messageId);

        void Save();
    }
}