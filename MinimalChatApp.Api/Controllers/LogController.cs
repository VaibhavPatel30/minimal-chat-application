using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.Business.IService;
using MinimalChatApp.Data;
using MinimalChatApp.Entity.DTOs;

namespace MinimalChatApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogService _logService;

        public LogController(AppDbContext dbContext, ILogService logService)
        {
            _dbContext = dbContext;
            _logService = logService;
        }

        //Get Request Logs
        [Authorize]
        [HttpGet]
        [Route("log")]
        public async Task<IActionResult> GetLogs([FromQuery] LogRequest request)
        {
            var end = request.EndTime ?? DateTime.UtcNow;
            var start = request.StartTime ?? end.AddMinutes(-5);

            if (start > end)
                return BadRequest(new { error = "Invalid time range" });

            var logs = await _logService.RequestLog(end, start);

            if (logs.Count == 0)
                return NotFound(new { error = "No logs found" });

            return Ok(new { logs });
        }
    }
}
