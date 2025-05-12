using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Data.IRepository;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly AppDbContext _dbContext;

        public LogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<RequestLog>> Log(DateTime? EndDate, DateTime? StartDate)
        {
            return await _dbContext.RequestLogs
                .Where(l => l.TimeOfCall >= StartDate && l.TimeOfCall <= EndDate)
                .OrderByDescending(l => l.TimeOfCall)
                .ToListAsync();
        }
    }
}
