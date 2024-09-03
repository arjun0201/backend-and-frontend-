using ShopAdminTool.Api.Resources;

namespace ShopAdminTool.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        public const string ApiKeyName = "ApiKey";
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(ErrorMessages.ApiKeyAbsent);
                _logger.LogError(ErrorMessages.ApiKeyAbsent);
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            var apiKey = appSettings.GetValue<string>(ApiKeyName);

            if (apiKey==null || !apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(ErrorMessages.UnauthorizedClient);
                _logger.LogError(ErrorMessages.UnauthorizedClient);
                return;
            }

            await _next(context);
        }
    }
}