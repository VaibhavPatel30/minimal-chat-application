using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class ForwardMessageRequest
    {
        public Guid OriginalMessageId { get; set; }         // ID of the message to forward
        public Guid ForwardToId { get; set; }               // UserId or GroupId to forward to
        public bool IsGroup { get; set; }                   // True if forwarding to group
    }
}
