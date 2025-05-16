using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class AddMemberResponse
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
    }
}
