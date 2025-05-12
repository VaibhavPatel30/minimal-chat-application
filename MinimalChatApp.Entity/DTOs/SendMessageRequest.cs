using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class SendMessageRequest
    {
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
