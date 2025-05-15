using MinimalChatApp.Data;
using MinimalChatApp.Entity.Models;
using System.Text;

namespace MinimalChatApp.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            context.Request.EnableBuffering();

            var body = "";
            if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
            {
                var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
                await context.Request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
                body = Encoding.UTF8.GetString(buffer);
                context.Request.Body.Position = 0;
            }

            var ip = context.Connection.RemoteIpAddress?.ToString();
            string? username = null;

            if (context.User.Identity?.IsAuthenticated == true)
            {
                username = context.User.Identity.Name;
            }

            var log = new RequestLog
            {
                IPAddress = ip,
                RequestBody = body,
                UserName = username,
                Path = context.Request.Path
            };

            await dbContext.RequestLogs.AddAsync(log);
            await dbContext.SaveChangesAsync();

            await _next(context);
        }
    }
}
