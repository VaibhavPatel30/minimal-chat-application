using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Entity.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserProfileDto Profile { get; set; }
    }

    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public PresenceStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastSeen { get; set; }
        public string CustomStatusMessage { get; set; }
        public DateTime? StatusStartDate { get; set; }
        public DateTime? StatusEndDate { get; set; }
    }
}
