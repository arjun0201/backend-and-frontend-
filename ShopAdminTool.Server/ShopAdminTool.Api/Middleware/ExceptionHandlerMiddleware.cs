using ShopAdminTool.Api.Resources;
using ShopAdminTool.Core.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace ShopAdminTool.Api.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
    
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;


        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string errorMessage;
            
            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorMessage = exception.Message;
                break;
                case AleadyExistsException:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorMessage = exception.Message;
                break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorMessage = ErrorMessages.GeneralExceptionMessage;
                break;
            }

            context.Response.ContentType = Text.Plain;
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(errorMessage);

            _logger.LogError(exception, exception.Message);
        }
    }
}