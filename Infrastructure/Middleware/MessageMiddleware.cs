using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Infrastructure.Middleware
{
    public class MessageMiddleware
    {
        private ILogger<MessageMiddleware> _logger;
        private readonly RequestDelegate _next;

        public MessageMiddleware(ILogger<MessageMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("x-app-id", out var source);
            _logger.LogInformation($"> Incoming message from {source}");
            await _next(httpContext);
        }
    }
}
