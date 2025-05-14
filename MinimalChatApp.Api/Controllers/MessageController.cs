using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Chathub;
using MinimalChatApp.Entity.DTOs;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        //Send Message
        [Authorize]
        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> SendMessage(SendMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Message sending failed due to validation errors" });

            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var senderName =User.FindFirst(ClaimTypes.Name)?.Value!;

            if (string.IsNullOrEmpty(senderId.ToString()))
            {
                return Unauthorized(new { error = "Unauthorized access" });
            }

            var result = await _messageService.SendMessageAsync(senderId, senderName, request);

            if (result == null)
                return BadRequest(new { error = "Invalid receiver or message content" });

            // Realtime push to receiver via SignalR
            var receiverId = result.ReceiverId.ToString();
            await _hubContext.Clients.User(receiverId).SendAsync("ReceiveMessage", result);

            return Ok(result);
        }


        //Edit Message
        [Authorize]
        [HttpPut]
        [Route("{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, EditMessageRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Message editing failed due to validation errors" });

            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var (success, error) = await _messageService.EditMessageAsync(senderId, messageId, request.Content);

            if (!success)
            {
                if (error == "Message not found")
                    return NotFound(new { error });

                if (error == "Unauthorized to edit this message")
                    return Unauthorized(new { error });

                return BadRequest(new { error });
            }

            return Ok(new { message = "Message edited successfully" });
        }


        //Delete Message
        [Authorize]
        [HttpDelete]
        [Route("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var (success, error) = await _messageService.DeleteMessageAsync(senderId, messageId);

            if (!success)
            {
                if (error == "Message not found")
                    return NotFound(new { error });

                if (error == "Unauthorized to delete this message")
                    return Unauthorized(new { error });

                return BadRequest(new { error });
            }

            return Ok(new { message = "Message deleted successfully" });
        }


        //Get Messages
        [Authorize]
        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> GetConversation(Guid userId, DateTime? before, int count = 20, string sort = "asc")
        {
            if (userId == Guid.Empty || (sort.ToLower() != "asc" && sort.ToLower() != "desc"))
                return BadRequest(new { error = "Invalid request parameters" });

            var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var beforeTimestamp = before ?? DateTime.UtcNow;

            var messages = await _messageService.GetConversationAsync(currentUserId, userId, beforeTimestamp, count, sort);

            if (messages == null || messages.Count == 0)
                return NotFound(new { error = "User or conversation not found" });

            var response = messages.Select(m => new
            {
                id = m.MessageId,
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                content = m.Content,
                timestamp = m.Timestamp
            });

            return Ok(new { messages = response });
        }


        //Search Messages
        [Authorize]
        [HttpGet]
        [Route("conversation/search")]
        public async Task<IActionResult> SearchMessages([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query parameter is required" });

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);// Custom extension to get user ID from JWT

            var messages = await _messageService.GetConversationByContentAsync(userId, query);

            if (messages == null || messages.Count == 0)
                return NotFound(new { error = "conversation not found with matching query" });

            var response = messages.Select(m => new
            {
                id = m.MessageId,
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                content = m.Content,
                timestamp = m.Timestamp
            });

            return Ok(new { messages = response });

        }

    }
}
