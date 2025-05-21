using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace MinimalChatApp.Entity.DTOs
{
    public class SendGroupMessageRequest
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public string Content { get; set; }
        public IFormFile? Attachment { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
