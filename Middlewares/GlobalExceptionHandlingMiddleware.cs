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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string errorId = Guid.NewGuid().ToString();
            var errorResponse = new ErrorResponse
            {
                Source = exception.TargetSite?.DeclaringType?.FullName,
                Exception = exception.Message,
                ErrorId = errorId
            };
            errorResponse.Messages.Add(exception.Message);

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
                    errorResponse.StatusCode = (int)e.StatusCode;
                    if (e.Messages is not null)
                    {
                        errorResponse.Messages = e.Messages;
                    }

                    break;

                case KeyNotFoundException:
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.Log(LogLevel.Error, $"{errorResponse.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
            var response = context.Response;
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = errorResponse.StatusCode;
                await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Can't write error response. Response has already started.");
            }
        }
    }
}