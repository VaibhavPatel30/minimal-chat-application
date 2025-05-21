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

        //Find and return user details by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByGuidAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OtherUserResponse>> GetAllUsersAsync()
        {

            return await _context.Users
        .Select(u => new OtherUserResponse
        {
            UserId = u.UserId,
            Name = u.Name,
            Email = u.Email,
            Status = u.Status,
            CustomStatusMessage = u.CustomStatusMessage,
            StatusStartDate = u.StatusStartDate,
            StatusEndDate = u.StatusEndDate,
            IsActive = u.IsActive,
            LastSeen = u.LastSeen,

        }).OrderByDescending(u => u.IsActive)               // Active users first
          .ThenByDescending(u => u.LastSeen)
        .ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

    }
}
