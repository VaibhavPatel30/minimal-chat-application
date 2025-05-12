using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.IService
{
    public interface ILogService
    {
        Task<List<RequestLog>> RequestLog(DateTime? EndDate, DateTime? StartDate);
    }
}
