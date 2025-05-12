using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Business.Service
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<List<RequestLog>> RequestLog(DateTime? EndDate, DateTime? StartDate)
        {
            return await _logRepository.Log(EndDate, StartDate);
        }
    }
}
