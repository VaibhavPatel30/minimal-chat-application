using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace MinimalChatApp.Entity.DTOs
{
    public class SendMessageRequest
    {
        public Guid ReceiverId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        public IFormFile? Attachment { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
