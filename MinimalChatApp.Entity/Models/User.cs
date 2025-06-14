using System.ComponentModel.DataAnnotations;


namespace MinimalChatApp.Entity.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // New Status Fields
        public PresenceStatus Status { get; set; } = PresenceStatus.Online;

        public bool IsActive { get; set; } = false;

        public DateTime? LastSeen { get; set; }

        public string? CustomStatusMessage { get; set; }

        public DateTime? StatusStartDate { get; set; }

        public DateTime? StatusEndDate { get; set; }


        // Navigation properties
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    }

    public enum PresenceStatus
    {
        Online,
        Offline,
        Away,
        DoNotDisturb
    }
}
