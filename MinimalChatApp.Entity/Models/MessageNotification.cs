using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.Models
{
    public class MessageNotification
    {
        public int Id { get; set; }

        public Guid? MessageId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public Message Message { get; set; } 

        public Guid? RecieverId { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
