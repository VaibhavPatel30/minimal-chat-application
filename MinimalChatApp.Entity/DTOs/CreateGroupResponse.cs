using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class CreateGroupResponse
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
