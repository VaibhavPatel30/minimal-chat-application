using MinimalChatApp.Entity.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MinimalChatApp.Data.IRepository
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message);
        Task<Message?> GetByIdAsync(Guid messageId);
        Task UpdateAsync(Message message);
        Task DeleteAsync(Message message);
        Task<List<Message>> GetConversationAsync(Guid userId1, Guid userId2, DateTime before, int count, string sort);
        Task<List<Message>> GetConversationByContentAsync(Guid userId, string query);

        Task AddGroupMessageAsync(GroupMessage groupMessage);
        Task<bool> GenerateNotificationAsync(Guid? ReceiverId, Guid? MessageId);

    }
}
