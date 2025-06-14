using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Entity.DTOs
{
    public class UpdateStatusRequest
    {
        public PresenceStatus Status { get; set; }

        public string? CustomMessage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
