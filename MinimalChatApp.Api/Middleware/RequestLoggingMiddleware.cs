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

            string body = string.Empty;

            if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
            {
                var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
                await context.Request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
                context.Request.Body.Position = 0;

                if (IsTextContent(context.Request.ContentType))
                {
                    body = Encoding.UTF8.GetString(buffer);

                    // Remove null characters to avoid PostgreSQL errors
                    body = body.Replace("\0", string.Empty);
                }
                else
                {
                    // Avoid storing raw binary data in text fields
                    body = "[Non-text body content omitted]";
                }
            }

            var ip = context.Connection.RemoteIpAddress?.ToString();
            string? username = context.User.Identity?.IsAuthenticated == true
                ? context.User.Identity?.Name
                : null;

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

        private bool IsTextContent(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return false;

            return contentType.Contains("application/json") ||
                   contentType.Contains("text/plain") ||
                   contentType.Contains("application/xml") ||
                   contentType.Contains("application/x-www-form-urlencoded");
        }
    }
}
