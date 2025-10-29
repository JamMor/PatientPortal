using System.Collections.Generic;
using System.Linq;
using PatientPortal.DTOs;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessagingService
    {
        //COMMANDS
        
        void CreateConversation(int linkId, ConversationDTO newConversationDTO, MessageDTO firstMessageDTO);
        void CreateReply(int linkId, int conversationId, MessageDTO newReply);
        bool MarkRead(int linkId, int messageId);

        //QUERIES

        MessagingLink? GetMessagingLink(int linkId);
        int GetUnreadTotalCount(MessagingLink messagingLink);
        int GetUnreadPatientCount(MessagingLink messagingLink);
        Recipient? GetPatientRecipient(int? toLinkId);
        List<Recipient> GetAllOtherStaffAsRecipients(int linkId, int? toLinkId);
        IQueryable<Conversation> GetPatientConversations(int linkId, bool onlyUnread);
        IQueryable<Conversation> GetStaffConversations(int linkId, bool onlyUnread);

    }
}