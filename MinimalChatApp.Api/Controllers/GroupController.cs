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
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMessageService _messageService;
        public GroupController(IGroupService groupService, IHubContext<ChatHub> hubContext, IMessageService messageService)
        {
            _groupService = groupService;
            _hubContext = hubContext;
            _messageService = messageService;
        }


        //Create Group
        [Authorize]
        [HttpPost]
        [Route("group")]
        public async Task<IActionResult> CreateGroup([FromBody] string groupName)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = "Group creation failed due to validation errors" });
                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                if (string.IsNullOrEmpty(currentUser.ToString()))
                {
                    return Unauthorized(new { error = "Unauthorized access" });
                }
                var response = await _groupService.CreateGroupAsync(groupName, currentUser);
                return Ok(response);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }


        //Update Group Name
        [Authorize]
        [HttpPut]
        [Route("group")]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Group modification failed due to validation errors" });
                }
                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _groupService.UpdateGroupAsync(request.GroupId, request.NewGroupName, currentUser);
                return Ok(result);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }


        //Detele Group
        [Authorize]
        [HttpDelete]
        [Route("group")]
        public async Task<IActionResult> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Group deletion failed due to validation errors" });
                }
                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var isGroupDeleted = await _groupService.DeleteGroupAsync(request.GroupId, request.GroupName, currentUser);
                if (isGroupDeleted)
                {
                    return Ok(new { message = "Group deleted successfully" });
                }
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return StatusCode(500, "Internal Error occured while deleting group.");
        }


        //Add Member to Group
        [Authorize]
        [HttpPost]
        [Route("member")] //also responsible to share conversation history.
        public async Task<IActionResult> AddMember([FromBody] AddMemberRequest request)
        {
            if (!ModelState.IsValid || (request.AccessType == MessageAccessType.Days && (!request.Days.HasValue || request.Days <= 0)))
            {
                return BadRequest(new { error = "Adding member failed due to validation errors" });
            }

            try
            {
                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _groupService.AddMemberAsync(request.UserId, request.GroupId, currentUser, request.AccessType, request.Days);
                return Ok(result);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //Delete Member from Group
        [Authorize]
        [HttpDelete]
        [Route("member")]
        public async Task<IActionResult> RemoveMember([FromBody] int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Member deletion failed due to validation errors" });

            try
            {
                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                await _groupService.RemoveMemberAsync(id, currentUser);
                return Ok(new { message = "Member deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //Send Message to Group
        [Authorize]
        [HttpPost]
        [Route("groupmessages")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendGroupMessage([FromForm] SendGroupMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Message sending failed due to validation errors" });

            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var senderName = User.FindFirst(ClaimTypes.Name)?.Value!;

            try
            {
                var memberIds = await _groupService.GetMemberUserIdsByGroupIdAsync(request.GroupId);
                var result = await _groupService.SendMessageToGroupAsync(request.GroupId, request.Content, request.Attachment, request.ParentMessageId, senderId, senderName);

                foreach (var userId in memberIds)
                {
                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", new
                    {
                        messageId = result.MessageId,
                        request.GroupId,
                        senderId,
                        request.Content,
                        timestamp = result.Timestamp
                    });
                    var isNotificationSent = await _messageService.GenerateNotificationAsync(userId, result.MessageId);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //Retrive Group Messages
        [Authorize]
        [HttpGet]
        [Route("groupmessages")]
        public async Task<IActionResult> GetConversation(Guid groupId, DateTime? before, int count = 20, string sort = "asc")
        {
            try
            {
                if (groupId == Guid.Empty || (sort.ToLower() != "asc" && sort.ToLower() != "desc"))
                    return BadRequest(new { error = "Invalid request parameters" });

                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var beforeTimestamp = before ?? DateTime.UtcNow;

                var messages = await _groupService.GetConversationAsync(currentUser, groupId, beforeTimestamp, count, sort);

                if (messages == null || messages.Count == 0)
                    return NotFound(new { error = "conversation not found" });

                var response = messages.Select(m => new
                {
                    id = m.MessageId,
                    senderId = m.SenderId,
                    content = m.Content,
                    timestamp = m.Timestamp
                });

                return Ok(new { messages = response });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }


        //Search Message in Group by Content
        [Authorize]
        [HttpGet]
        [Route("groupconversation/search")]
        public async Task<IActionResult> SearchMessages([FromQuery] Guid groupId, [FromQuery] string query)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(query) || groupId == null)
                    return BadRequest(new { error = "Invalid request parameters" });

                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                var messages = await _groupService.GetConversationByContentAsync(currentUser, groupId, query);

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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }

        }
    }
}
