using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> GetByGuidAsync(string userId);
        Task AddUserAsync(User user);
        Task<List<OtherUserResponse>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
    }
}
