using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Entity.DTOs
{
    public class SendMessageRequest
    {
        public Guid ReceiverId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
    }
}
