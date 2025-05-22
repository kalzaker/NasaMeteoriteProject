using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace NasaMeteoriteService.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred while processing the request.");

            var (statusCode, message) = exception switch
            {
                ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = new
                {
                    message,
                    statusCode = (int)statusCode
                }
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);

            return true;
        }
    }
}
