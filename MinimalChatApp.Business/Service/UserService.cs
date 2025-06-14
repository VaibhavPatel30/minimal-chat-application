using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business.ExceptionHandlers;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        //Register new user
        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ConflictException("Registration failed because the email is already registered");


            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                PasswordHash = passwordHash
            };

            await _userRepository.AddUserAsync(user);

            return new UserResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            // Mark user as active and clear last seen
            user.IsActive = true;
            user.LastSeen = null;
            await _userRepository.UpdateUserAsync(user);

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                Profile = new UserProfileDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Status = user.Status,
                    IsActive = true,
                    LastSeen = null,
                    CustomStatusMessage = user.CustomStatusMessage,
                    StatusStartDate = user.StatusStartDate,
                    StatusEndDate = user.StatusEndDate
                }
            };
        }

        public async Task<object?> GoogleLoginAsync(ClaimsPrincipal principal)
        {
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;

            //if (string.IsNullOrEmpty(email)) return null;
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = email,
                    Name = name,
                    PasswordHash = "hash"
                    // optionally set source = "Google"
                };
                await _userRepository.AddUserAsync(user);
            }

            var token = GenerateJwtToken(user);

            return new
            {
                token,
                profile = new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.Status,
                    user.CustomStatusMessage,
                    user.StatusStartDate,
                    user.StatusEndDate
                }
            };
        }

        public async Task<List<OtherUserResponse>> GetAllUsersExceptAsync(string currentUser)
        {
            List<OtherUserResponse> users = await _userRepository.GetAllUsersAsync();
            return users
                     .Where(x => x.UserId.ToString() != currentUser)
                    .ToList();
        }

        public async Task UpdateStatusAsync(UpdateStatusRequest request, Guid currentUser)
        {
            var userDetails = await _userRepository.GetByGuidAsync(currentUser.ToString());

            if (userDetails == null)
            {
                throw new NotFoundException("User not Found");
            }

            userDetails.Status = request.Status;
            userDetails.CustomStatusMessage = request.CustomMessage;
            userDetails.StatusStartDate = request.StartDate;
            userDetails.StatusEndDate = request.EndDate;

            await _userRepository.UpdateUserAsync(userDetails);
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _userRepository.GetByGuidAsync(userId.ToString());
            if (user != null)
            {
                user.IsActive = false;
                user.LastSeen = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
