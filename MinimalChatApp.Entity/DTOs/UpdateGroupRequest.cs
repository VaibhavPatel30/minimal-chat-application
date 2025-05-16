using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class UpdateGroupRequest
    {
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        public string NewGroupName { get; set; }
    }
}
