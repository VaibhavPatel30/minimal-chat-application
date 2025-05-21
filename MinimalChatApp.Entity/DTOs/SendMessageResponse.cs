using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class SendMessageResponse
    {
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; }
        public string? Attachment { get; set; }
        public string? AttachmentType { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid? ParentMessageId { get; set; }
        public Guid? ForwardedFromMessageId { get; set; } // NEW
    }
}
