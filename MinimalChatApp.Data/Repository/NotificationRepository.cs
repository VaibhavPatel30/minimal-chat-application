using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<MessageNotification>> GetNotificationByUserIdAsync(Guid UserId)
        {
            return await _context.MessageNotifications
                    .Include(n => n.Message) // Eager load message
                    .Where(n => n.RecieverId == UserId && !n.IsRead)
                    .ToListAsync();
        }

        public async Task<bool> ReadNotificationByIdAsync(Guid UserId, int NotificationId)
        {
            var notification = await _context.MessageNotifications
                    .FirstOrDefaultAsync(n => n.RecieverId == UserId && n.Id == NotificationId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
