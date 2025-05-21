using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.Service
{
    public class MessageService : IMessageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SendMessageResponse?> SendMessageAsync(Guid senderId, string senderName, SendMessageRequest request)
        {
            // Validate receiver
            var receiver = await _userRepository.GetByGuidAsync(request.ReceiverId.ToString());
            if (receiver == null)
                return null;

            string? fileUrl = null;
            string? fileType = null;

            if (request.Attachment != null)
            {
                var fileId = Guid.NewGuid();
                var fileName = $"{fileId}_{request.Attachment.FileName}";
                var filePath = Path.Combine("wwwroot/uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.Attachment.CopyToAsync(stream);

                var scheme = _httpContextAccessor.HttpContext?.Request.Scheme;
                var host = _httpContextAccessor.HttpContext?.Request.Host.Value;

                fileUrl = $"{scheme}://{host}/uploads/{fileName}";
                fileType = request.Attachment.ContentType;
            }

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Attachment = fileUrl,
                AttachmentType = fileType,
                Timestamp = DateTime.UtcNow,
                ParentMessageId = request.ParentMessageId
            };

            var result = await _messageRepository.CreateAsync(message);

            return new SendMessageResponse
            {
                MessageId = result.MessageId,
                SenderId = result.SenderId,
                SenderName = result.SenderName,
                ReceiverId = result.ReceiverId,
                Content = result.Content,
                Attachment = result.Attachment,
                AttachmentType = result.AttachmentType,
                Timestamp = result.Timestamp,
                ParentMessageId = result.ParentMessageId

            };
        }

        public async Task<bool> EditMessageAsync(Guid senderId, Guid messageId, string newContent)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new NotFoundException("Message not found");
            }

            if (message.SenderId != senderId)
            {
                throw new UnauthorizedAccessException("Unauthorized to edit this message");
            }
            if (string.IsNullOrWhiteSpace(newContent))
            {
                throw new Exception("Message content cannot be empty");
            }
            message.Content = newContent;
            await _messageRepository.UpdateAsync(message);

            return true;
        }

        public async Task<bool> DeleteMessageAsync(Guid senderId, Guid messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new NotFoundException("Message not found");
            }

            if (message.SenderId != senderId)
            {
                throw new UnauthorizedAccessException("Unauthorized to Delete this message");
            }

            await _messageRepository.DeleteAsync(message);
            return true;
        }

        public async Task<List<Message>> GetConversationAsync(Guid currentUserId, Guid otherUserId, DateTime before, int count, string sort)
        {
            return await _messageRepository.GetConversationAsync(currentUserId, otherUserId, before, count, sort);
        }

        public async Task<List<Message>> GetConversationByContentAsync(Guid userId, string query)
        {
            return await _messageRepository.GetConversationByContentAsync(userId, query);
        }

        public async Task<bool> GenerateNotificationAsync(Guid? ReceiverId, Guid? MessageId)
        {
            return await _messageRepository.GenerateNotificationAsync(ReceiverId, MessageId);
        }

        public async Task<SendMessageResponse?> ForwardMessageAsync(Guid senderId, string senderName, ForwardMessageRequest request)
        {
            var originalMessage = await _messageRepository.GetByIdAsync(request.OriginalMessageId);
            if (originalMessage == null)
                throw new NotFoundException("Original message not found");

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = request.ForwardToId,
                Content = $"[Forwarded]\n{originalMessage.Content}",
                Attachment = originalMessage.Attachment,
                AttachmentType = originalMessage.AttachmentType,
                Timestamp = DateTime.UtcNow,
                ForwardedFromMessageId = request.OriginalMessageId
            };

            await _messageRepository.CreateAsync(message);
            if (request.IsGroup)
            {
                // Save GroupMessage mapping
                var groupMessage = new GroupMessage
                {
                    GroupId = request.ForwardToId,
                    MessageId = message.MessageId
                };
                await _messageRepository.AddGroupMessageAsync(groupMessage);
            }

            return new SendMessageResponse
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                ReceiverId = message.ReceiverId ?? Guid.Empty,
                Content = message.Content,
                Attachment = message.Attachment,
                AttachmentType = message.AttachmentType,
                Timestamp = message.Timestamp,
                ParentMessageId = message.ParentMessageId,
                ForwardedFromMessageId = message.ForwardedFromMessageId 
            };
        }
    }
}
