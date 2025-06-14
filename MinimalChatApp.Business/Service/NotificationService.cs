using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task<List<MessageNotificationResponse>> GetNotificationByUserIdAsync(Guid UserId)
        {
            var notifications = await _notificationRepository.GetNotificationByUserIdAsync(UserId);
            var result = notifications.Select(n => new MessageNotificationResponse
            {
                Id = n.Id,
                MessageId = n.MessageId,
                RecieverId = n.RecieverId,
                SenderName = n.Message.SenderName,
                Content = n.Message.Content,
                IsRead = n.IsRead,
            }).ToList();

            return result;
        }

        public async Task<bool> ReadNotificationByIdAsync(Guid UserId, int NotificationId)
        {

            bool isRead = await _notificationRepository.ReadNotificationByIdAsync(UserId, NotificationId);
            if (!isRead)
            {
                throw new NotFoundException("Notification not found");
            }
            return isRead;
        }
    }
}
