using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.IRepository
{
    public interface INotificationRepository
    {
        Task<List<MessageNotification>> GetNotificationByUserIdAsync(Guid UserId);
        Task<bool> ReadNotificationByIdAsync(Guid UserId, int NotificationId);
    }
}
