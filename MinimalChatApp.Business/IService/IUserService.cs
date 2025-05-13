using System.Security.Claims;
using MinimalChatApp.Entity.DTOs;

namespace MinimalChatApp.Business.IService
{
    public interface IUserService
    {
        Task<(bool IsSuccess, string? Error, UserResponse? Response)> RegisterAsync(RegisterRequest request);

        Task<LoginResponse?> LoginAsync(LoginRequest request);

        List<UserResponse> GetAllUsersExcept(string currentUser);

        Task<object?> GoogleLoginAsync(ClaimsPrincipal principal);
    }
}
