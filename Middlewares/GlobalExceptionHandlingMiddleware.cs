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
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Success = false
            };

            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid Token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        errorResponse.StatusCode = response.StatusCode;
                        errorResponse.Message = ex.Message;
                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = ex.Message;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Internal Server Error";
                    break;
            }
            _logger.LogError(exception.Message);
            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);

            //     switch (exception)
            //     {
            //         case ApplicationException ex:
            //             if (ex.Message.Contains("Invalid Token"))
            //             {
            //                 response.StatusCode = (int)HttpStatusCode.Forbidden;
            //                 result = JsonSerializer.Serialize(new
            //                 {
            //                     Type = "Forbidden Error",
            //                     Title = "Forbidden Error",
            //                     Detail = "Unauthorized"
            //                 });
            //                 break;
            //             }
            //             response.StatusCode = (int)HttpStatusCode.BadRequest;
            //             break;
            //         default:
            //             response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //             result = JsonSerializer.Serialize(new
            //             {
            //                 Type = "Server Error",
            //                 Title = "Internal Server Error",
            //                 Detail = "An internal server has occurred"
            //             });
            //             break;
            //     }
            //     _logger.LogError(exception, exception.Message);
            //     await response.WriteAsync(result);
            // }
        }
    }
}