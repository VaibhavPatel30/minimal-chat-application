using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.DTOs;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByGuidlAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public List<UserResponse> GetAllUsers()
        {

            return _context.Users
        .Select(u => new UserResponse
        {
            UserId = u.UserId,
            Name = u.Name,
            Email = u.Email
        })
        .ToList();
        }

    }
}
