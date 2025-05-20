using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        //Get all Notifications
        [Authorize]
        [HttpGet]
        [Route("notification")]
        public async Task<IActionResult> GetAllNotification()
        {
            try
            {
                var UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                List<MessageNotificationResponse> notifications = await _notificationService.GetNotificationByUserIdAsync(UserId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //Read Notification
        [Authorize]
        [HttpPut]
        [Route("notification")]
        public async Task<IActionResult> ReadNotification(int NotificationId)
        {
            try
            {
                var UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var isRead = await _notificationService.ReadNotificationByIdAsync(UserId, NotificationId);
                return Ok(isRead);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
