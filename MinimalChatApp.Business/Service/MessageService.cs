using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<SendMessageResponse?> SendMessageAsync(Guid senderId, SendMessageRequest request)
        {
            // Validate receiver
            var receiver = await _userRepository.GetByGuidlAsync(request.ReceiverId.ToString());
            if (receiver == null)
                return null;

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Timestamp = DateTime.UtcNow
            };

            var result = await _messageRepository.CreateAsync(message);

            return new SendMessageResponse
            {
                MessageId = result.MessageId,
                SenderId = result.SenderId,
                ReceiverId = result.ReceiverId,
                Content = result.Content,
                Timestamp = result.Timestamp
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> EditMessageAsync(Guid senderId, Guid messageId, string newContent)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
                return (false, "Message not found");

            if (message.SenderId != senderId)
                return (false, "Unauthorized to edit this message");

            if (string.IsNullOrWhiteSpace(newContent))
                return (false, "Message content cannot be empty");

            message.Content = newContent;
            await _messageRepository.UpdateAsync(message);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteMessageAsync(Guid senderId, Guid messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
                return (false, "Message not found");

            if (message.SenderId != senderId)
                return (false, "Unauthorized to delete this message");

            await _messageRepository.DeleteAsync(message);
            return (true, null);
        }

        public async Task<List<Message>> GetConversationAsync(Guid currentUserId, Guid otherUserId, DateTime before, int count, string sort)
        {
            return await _messageRepository.GetConversationAsync(currentUserId, otherUserId, before, count, sort);
        }

        public async Task<List<Message>> GetConversationByContentAsync(Guid userId, string query)
        {
            return await _messageRepository.GetConversationByContentAsync(userId, query);
        }
    }
}
