using System.Net;
using System.Text.Json;

namespace griffined_api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problemJson = new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "Server Error",
                    Title = "Error",
                    Detail = "An internal server has occurred"
                };

                string problem = JsonSerializer.Serialize(problemJson);

                await context.Response.WriteAsync(problem);

                context.Response.ContentType = "application/json";
            }
        }
    }
}