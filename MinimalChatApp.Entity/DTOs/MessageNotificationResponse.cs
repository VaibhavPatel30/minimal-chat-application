using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class MessageNotificationResponse
    {
        public int Id { get; set; }
        public Guid? MessageId { get; set; }
        public Guid? RecieverId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }

    }
}
