using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Entity.DTOs
{
    public class AddMemberResponse
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public MessageAccessType AccessType { get; set; }
        public int? Days { get; set; }
    }
}
