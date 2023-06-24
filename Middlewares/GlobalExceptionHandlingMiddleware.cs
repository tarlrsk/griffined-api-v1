using System.Net;
using System.Text.Json;
using griffined_api.Models;

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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string errorId = Guid.NewGuid().ToString();
            var errorResult = new ErrorResponse
            {
                Source = exception.TargetSite?.DeclaringType?.FullName,
                Exception = exception.Message,
                ErrorId = errorId
            };
            errorResult.Messages.Add(exception.Message);

            if (exception is not CustomException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            switch (exception)
            {
                case CustomException e:
                    errorResult.StatusCode = (int)e.StatusCode;
                    if (e.Messages is not null)
                    {
                        errorResult.Messages = e.Messages;
                    }

                    break;

                case KeyNotFoundException:
                    errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.Log(LogLevel.Error, $"{errorResult.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
            var response = context.Response;
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = errorResult.StatusCode;
                await response.WriteAsync(JsonSerializer.Serialize(errorResult));
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Can't write error response. Response has already started.");
            }
        }
    }
}