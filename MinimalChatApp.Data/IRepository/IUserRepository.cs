using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User> GetByGuidlAsync(string userId);
        Task AddAsync(User user);

        List<UserResponse> GetAllUsers();
    }
}
