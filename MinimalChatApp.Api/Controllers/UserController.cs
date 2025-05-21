using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
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
                return Ok(new { message = $"Status set to {request.Status}" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new {error = ex.Message});
            }

            
        }
    }
}
