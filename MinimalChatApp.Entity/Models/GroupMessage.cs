using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace MinimalChatApp.Entity.Models
{
    public class GroupMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }

        [Required]
        public Guid MessageId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public Message Message { get; set; }
    }
}
