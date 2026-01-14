using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Stamps.Web.Middleware;

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
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "You are not authorized to access this resource"),
            ArgumentException argEx => (HttpStatusCode.BadRequest, "Invalid Input", argEx.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Not Found", "The requested resource was not found"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid Operation", exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Server Error", "An unexpected error occurred. Please try again later.")
        };

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail,
            instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails),
            cancellationToken);

        return true;
    }
}

