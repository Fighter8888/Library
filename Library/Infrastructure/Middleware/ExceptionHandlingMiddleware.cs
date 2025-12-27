using System.Net;
using System.Text.Json;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Models;

namespace Library.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new ApiResponse<object>
        {
            Success = false
        };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                break;

            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = validationEx.Message;
                errorResponse.Errors = validationEx.Errors;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = invalidOpEx.Message;
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argEx.Message;
                break;

            default:
                // Security: Don't expose internal error details in production
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = _environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request";
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.Errors = new Dictionary<string, string[]>
                    {
                        { "StackTrace", new[] { exception.StackTrace ?? string.Empty } }
                    };
                }
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(json);
    }
}

