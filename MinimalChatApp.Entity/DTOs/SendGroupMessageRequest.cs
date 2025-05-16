using System.ComponentModel.DataAnnotations;


namespace MinimalChatApp.Entity.DTOs
{
    public class SendGroupMessageRequest
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
