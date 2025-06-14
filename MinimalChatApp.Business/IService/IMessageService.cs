using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MinimalChatApp.Business.IService
{
    public interface IMessageService
    {
        Task<SendMessageResponse?> SendMessageAsync(Guid senderId, string senderName, SendMessageRequest request);
        Task<bool> EditMessageAsync(Guid senderId, Guid messageId, string newContent);
        Task<bool> DeleteMessageAsync(Guid senderId, Guid messageId);
        Task<List<Message>> GetConversationAsync(Guid currentUserId, Guid otherUserId, DateTime before, int count, string sort);
        Task<List<Message>> GetConversationByContentAsync(Guid userId, string query);
        Task<bool> GenerateNotificationAsync(Guid? ReceiverId, Guid? MessageId);
        Task<SendMessageResponse> ForwardMessageAsync(Guid SenderId, string SenderName, ForwardMessageRequest request);
    }
}
