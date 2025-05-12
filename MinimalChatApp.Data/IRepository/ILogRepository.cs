using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.IRepository
{
    public interface ILogRepository
    {
        Task<List<RequestLog>> Log(DateTime? EndDate, DateTime? StartDate);
    }
}
