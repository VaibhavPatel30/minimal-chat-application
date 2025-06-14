using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.Models
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User? Sender { get; set; }

        public Guid? ForwardedFromMessageId { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public Guid? ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }

        public string? Attachment { get; set; } // File URL or path

        public string? AttachmentType { get; set; } // MIME type (e.g., image/png, application/pdf)

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Guid? ParentMessageId { get; set; }
    }
}
