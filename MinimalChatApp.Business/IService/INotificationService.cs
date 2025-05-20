using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.IService
{
    public interface INotificationService
    {
        Task<List<MessageNotificationResponse>> GetNotificationByUserIdAsync(Guid UserId);
        Task<bool> ReadNotificationByIdAsync(Guid UserId, int NotificationId);
    }
}
