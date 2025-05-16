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

        public User User { get; set; }
        public Group Group { get; set; }
    }
}
