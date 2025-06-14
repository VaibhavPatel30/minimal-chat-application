using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class SendGroupMessageResponse
    {
        public Guid MessageId { get; set; }
        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; }
        public string? Attachment { get; set; } // URL to file
        public string? AttachmentType { get; set; } // MIME type
        public DateTime Timestamp { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
