using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;


namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        //Register User
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Registration failed due to validation errors" });

            var (IsSuccess, Error, Response) = await _userService.RegisterAsync(request);

            if (!IsSuccess)
            {
                if (Error?.Contains("already registered") == true)
                    return Conflict(new { error = Error });

                return BadRequest(new { error = Error });
            }

            return Ok(Response);
        }


        //Login User
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Login failed due to validation errors" });

            var response = await _userService.LoginAsync(request);

            if (response == null)
                return Unauthorized(new { error = "Login failed due to incorrect credentials" });

            return Ok(response);
        }


        //Login Google
        [HttpGet]
        [Route("login/google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, "Google");
        }

        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Google");

            if (!result.Succeeded)
                return Unauthorized(new { error = "Google authentication failed" });

            var response = await _userService.GoogleLoginAsync(result.Principal);

            if (response == null)
                return Unauthorized(new { error = "Google login failed" });

            return Ok(response);
        }


        //Get User : exclude the loggedIn user!!!
        [Authorize]
        [HttpGet]
        [Route("users")]
        public ActionResult GetAllUsers()
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUser))
            {
                return Unauthorized(new {error = "Unauthorized access"});
            }

            var users = _userService.GetAllUsersExcept(currentUser);

            return Ok(new { users });
        }
    }
}
