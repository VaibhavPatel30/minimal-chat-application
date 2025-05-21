using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> CreateAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message?> GetByIdAsync(Guid messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Message message)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetConversationAsync(Guid userId1, Guid userId2, DateTime before, int count, string sort)
        {
            var query = await _context.Messages
                .Where(m =>
                    ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                     (m.SenderId == userId2 && m.ReceiverId == userId1)) &&
                    m.Timestamp < before).AsNoTracking().ToListAsync();

            query = sort.ToLower() == "desc"
                ? query.OrderByDescending(m => m.Timestamp).ToList()
                : query.OrderBy(m => m.Timestamp).ToList();

            return query.Take(count).ToList();
        }

        public async Task<List<Message>> GetConversationByContentAsync(Guid userId, string query)
        {
            string loweredQuery = $"%{query.ToLower()}%";
            return await _context.Messages
                                .Where(m =>
                                    (m.SenderId == userId || m.ReceiverId == userId) &&
                                    EF.Functions.Like(m.Content.ToLower(), loweredQuery))
                                .OrderBy(m => m.Timestamp)
                                .ToListAsync();
        }

        public async Task AddGroupMessageAsync(GroupMessage groupMessage)
        {
            await _context.GroupMessages.AddAsync(groupMessage);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> GenerateNotificationAsync(Guid? ReceiverId, Guid? MessageId)
        {
            var notification = new MessageNotification
            {
                RecieverId = ReceiverId,
                MessageId = MessageId,
                IsRead = false
            };
            await _context.MessageNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
