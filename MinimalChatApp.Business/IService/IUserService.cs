using System.Security.Claims;
using MinimalChatApp.Entity.DTOs;

namespace MinimalChatApp.Business.IService
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse?> LoginAsync(LoginRequest request);

        Task<List<UserResponse>> GetAllUsersExceptAsync(string currentUser);

        Task<object?> GoogleLoginAsync(ClaimsPrincipal principal);
    }
}
