using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Entity.DTOs
{
    public class UserResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PresenceStatus Status { get; set; }
    }

    public class OtherUserResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PresenceStatus Status { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? LastSeen { get; set; }
        public string? CustomStatusMessage { get; set; }
        public DateTime? StatusStartDate { get; set; }
        public DateTime? StatusEndDate { get; set; }
    }
}
