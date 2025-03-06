using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using Microsoft.AspNetCore.Diagnostics;

namespace knowledge_sharing_platform_cloud.Exception
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            ApiResult<string> response;
            int statusCode = StatusCodes.Status500InternalServerError; // Default to Internal Server Error

            if (exception is BusinessException businessException)
            {
                response = ApiResult<string>.ServiceFail(businessException.ErrorCode, businessException.ErrorMsg);
                statusCode = StatusCodes.Status200OK; // Business exceptions are not HTTP errors
            }
            else if (exception is UnauthorizedAccessException) // Handle Unauthorized Access
            {
                response = ApiResult<string>.ServiceFail((int)CommonErrorEnum.UNAUTHORIZED, "Unauthorized access.");
                statusCode = StatusCodes.Status401Unauthorized; // Return 401 Unauthorized
            }
            else if (exception is ValidationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                response = ApiResult<string>.ServiceFail((int)CommonErrorEnum.PARAM_VALID, exception.Message);
            }
            else
            {
                response = ApiResult<string>.ServiceFail((int)CommonErrorEnum.SYSTEM_ERROR, exception.Message);
            }

            // Set response
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";
            var jsonResponse = JsonSerializer.Serialize(response);
            await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);

            return true;
        }

    }
}
