using System.Net;
using System.Text.Json;
using CardStore.Models;
using FluentValidation;

namespace CardStore.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        
        ApiResponse response;
        HttpStatusCode statusCode;

        switch (ex)
        {
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse.ErrorResult(
                    "Validation failed",
                    validationEx.Errors.Select(e => new { 
                        Property = e.PropertyName, 
                        Error = e.ErrorMessage 
                    })
                );
                _logger.LogWarning("Validation error: {Message}", validationEx.Message);
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                response = ApiResponse.ErrorResult("Unauthorized access");
                _logger.LogWarning("Unauthorized access attempt: {Message}", ex.Message);
                break;

            case InvalidOperationException invalidOpEx:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse.ErrorResult(invalidOpEx.Message);
                _logger.LogWarning("Invalid operation: {Message}", invalidOpEx.Message);
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                response = ApiResponse.ErrorResult("Resource not found");
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse.ErrorResult($"Invalid argument: {argEx.Message}");
                _logger.LogWarning("Invalid argument: {Message}", argEx.Message);
                break;

            case TimeoutException:
                statusCode = HttpStatusCode.RequestTimeout;
                response = ApiResponse.ErrorResult("Request timeout");
                _logger.LogWarning("Request timeout: {Message}", ex.Message);
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                response = ApiResponse.ErrorResult("An internal server error occurred");
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResponse = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(jsonResponse);
    }
}