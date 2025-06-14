using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.Models
{
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public MessageAccessType AccessType { get; set; }

        public int? Days { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Group Group { get; set; }
    }

    public enum MessageAccessType
    {
        None = 0,
        All = 1,
        Days = 2
    }
}
