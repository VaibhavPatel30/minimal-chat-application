using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class DeleteGroupRequest
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
    }
}
