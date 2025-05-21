using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Business.Service;
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

        /**
         * @api {post} /api/register Register a new user
         * @apiName RegisterUser
         * @apiGroup User
         *
         * @apiBody {String} Email User's email address (must be a valid email).
         * @apiBody {String} Name User's full name.
         * @apiBody {String} Password User's password.
         *
         * @apiSuccess {String} id Unique ID of the newly registered user.
         * @apiSuccess {String} name Name of the user.
         * @apiSuccess {String} email Email of the user.
         * @apiSuccess {String} token JWT token for authentication.
         *
         * @apiError (400 Bad Request) ValidationError Registration failed due to validation errors.
         * @apiError (409 Conflict) ConflictError Email already exists.
         */
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = "Registration failed due to validation errors" });

                UserResponse result = await _userService.RegisterAsync(request);
                return Ok(result);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }


        /**
         * @api {post} /api/login Login a user
         * @apiName LoginUser
         * @apiGroup User
         *
         * @apiBody {String} Email User's email address.
         * @apiBody {String} Password User's password.
         *
         * @apiSuccess {String} id Unique ID of the user.
         * @apiSuccess {String} name Name of the user.
         * @apiSuccess {String} email Email of the user.
         * @apiSuccess {String} token JWT token for authentication.
         *
         * @apiError (400 Bad Request) ValidationError Login failed due to validation errors.
         * @apiError (401 Unauthorized) AuthError Login failed due to incorrect credentials.
         */
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
        public async Task<IActionResult> GoogleLogin()
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
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            if (!result.Succeeded)
                return Unauthorized(new { error = "Google authentication failed" });

            var response = await _userService.GoogleLoginAsync(result.Principal);

            if (response == null)
                return Unauthorized(new { error = "Google login failed" });

            return Ok(response);
        }


        /**
         * @api {get} /api/users Get all users (excluding logged-in user)
         * @apiName GetAllUsers
         * @apiGroup User
         * @apiPermission Authenticated
         *
         * @apiHeader {String} Authorization Bearer token for authentication.
         *
         * @apiSuccess {Object[]} users List of users excluding the currently logged-in user.
         * @apiSuccess {String} users.id User ID.
         * @apiSuccess {String} users.name Name of the user.
         * @apiSuccess {String} users.email Email address of the user.
         *
         * @apiError (401 Unauthorized) UnauthorizedAccess User is not authenticated.
         */
        [Authorize]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUser))
            {
                return Unauthorized(new { error = "Unauthorized access" });
            }

            var users = await _userService.GetAllUsersExceptAsync(currentUser);

            return Ok(new { users });
        }



        [Authorize]
        [HttpPut]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var currentUser = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (string.IsNullOrEmpty(currentUser.ToString()))
            {
                return Unauthorized(new { error = "Unauthorized access" });
            }
            await _userService.LogoutAsync(currentUser);
            return Ok(new { message = "Logged out" });
            
        }
    }
}
