using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Chathub;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _hubContext;
        public UserController(IUserService userService, IHubContext<ChatHub> hubContext)
        {
            _userService = userService;
            _hubContext = hubContext;
        }


        [Authorize]
        [HttpPut]
        [Route("setstatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                if (string.IsNullOrEmpty(currentUser.ToString()))
                {
                    return Unauthorized(new { error = "Unauthorized access" });
                }

                await _userService.UpdateStatusAsync(request, currentUser);

                // Broadcast to all connected clients
                await _hubContext.Clients.All.SendAsync("StatusChanged", currentUser, request.Status);
                return Ok(new { message = $"Status set to {request.Status}" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }

            
        }
    }
}
