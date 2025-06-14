using System.Security.Claims;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.IService
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse?> LoginAsync(LoginRequest request);

        Task<List<OtherUserResponse>> GetAllUsersExceptAsync(string currentUser);

        Task<object?> GoogleLoginAsync(ClaimsPrincipal principal);

        Task UpdateStatusAsync(UpdateStatusRequest request, Guid currentUser);

        Task LogoutAsync(Guid userId);
    }
}
