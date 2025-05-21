using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Business.Service;
using MinimalChatApp.Chathub;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IGroupService _groupService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageService messageService,IGroupService groupService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _groupService = groupService;
            _hubContext = hubContext;
        }

        //Send Message
        [Authorize]
        [HttpPost]
        [Route("messages")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Message sending failed due to validation errors" });

            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var senderName = User.FindFirst(ClaimTypes.Name)?.Value!;

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

            //add to notification table
            var isNotificationSent = await _messageService.GenerateNotificationAsync(result.ReceiverId, result.MessageId);

            return Ok(result);
        }


        //Edit Message
        [Authorize]
        [HttpPut]
        [Route("{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] string Content)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Content))
                    return BadRequest(new { error = "Message editing failed due to validation errors" });

                var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                bool isMessageUpdated = await _messageService.EditMessageAsync(senderId, messageId, Content);
                if (isMessageUpdated)
                {
                    return Ok(new { message = "Message edited successfully" });
                }
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
            return StatusCode(500, "Internal Error occured while editing message.");

        }


        //Delete Message
        [Authorize]
        [HttpDelete]
        [Route("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            try
            {
                var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                bool isMessageDeleted = await _messageService.DeleteMessageAsync(senderId, messageId);
                if (isMessageDeleted)
                {
                    return Ok(new { message = "Message deleted successfully" });
                }

            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
            return StatusCode(500, "Internal Error occured while deleting message.");
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
                return NotFound(new { error = $"conversation not found with '{query}' word" });

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


        [Authorize]
        [HttpPost]
        [Route("forwardmessage")]
        public async Task<IActionResult> ForwardMessage([FromBody] ForwardMessageRequest request)
        {
            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);// Custom extension to get user ID from JWT
            var senderName = User.FindFirst(ClaimTypes.Name)?.Value!;
            var response = await _messageService.ForwardMessageAsync(senderId, senderName, request);
            //await _messageService.GenerateNotificationAsync(request.ForwardToId, response.MessageId);
            //Send message to signalR
            if (!request.IsGroup)
            {
                await _hubContext.Clients.User(request.ForwardToId.ToString()).SendAsync("ReceiveMessage", response);
                var isNotificationSent = await _messageService.GenerateNotificationAsync(request.ForwardToId, response.MessageId);
            }
            else
            {
                var memberIds = await _groupService.GetMemberUserIdsByGroupIdAsync(request.ForwardToId);
                foreach (var userId in memberIds)
                {
                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", new
                    {
                        messageId = response.MessageId,
                        request.ForwardToId,
                        senderId,
                        response.Content,
                        timestamp = response.Timestamp
                    });
                    var isNotificationSent = await _messageService.GenerateNotificationAsync(userId, response.MessageId);
                }
                await _hubContext.Clients.Group(request.ForwardToId.ToString()).SendAsync("ReceiveGroupMessage", response);
            }

            if (response == null)
                return NotFound("Message or user not found");

            return Ok(response);
        }

    }
}
