using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace WebApplication3
{
    public class RateLimiteMidleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimiteMidleware> _logger;
        private readonly IRateLimitService _rateLimitService;

        public RateLimiteMidleware(RequestDelegate next, ILogger<RateLimiteMidleware> logger, IRateLimitService rateLimitService)
        {
            _next = next;
            _logger = logger;
            _rateLimitService = rateLimitService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientQueryValues = context.Request.Query["clientId"];

            if (StringValues.IsNullOrEmpty(clientQueryValues))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                if (_rateLimitService.IsRateLimitReached(clientQueryValues, 5))
                {
                    _logger.LogWarning($"Rate Limit Reached for: Clientid:{clientQueryValues}");
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    await context.Response.WriteAsync("Rate Limit Exceeds");
                }
                else
                {
                    await _next(context);
                }
            }
        }
    }

    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseRateLimit(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimiteMidleware>();
        }
    }
}