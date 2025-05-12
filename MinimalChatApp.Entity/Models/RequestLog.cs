using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.Models
{
    public class RequestLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeOfCall { get; set; } = DateTime.UtcNow;
        public string? IPAddress { get; set; }
        public string? RequestBody { get; set; }
        public string? UserName { get; set; }
        public string? Path { get; set; }
    }
}
