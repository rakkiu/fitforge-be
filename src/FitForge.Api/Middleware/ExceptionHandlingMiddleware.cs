using System.Net;
using System.Text.Json;
using FitForge.Shared.Results;

namespace FitForge.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            ArgumentException ex =>
                (HttpStatusCode.BadRequest, Error.Validation("VALIDATION_ERROR", ex.Message)),
            UnauthorizedAccessException =>
                (HttpStatusCode.Unauthorized, Error.Unauthorized("UNAUTHORIZED", "Authentication required")),
            KeyNotFoundException ex =>
                (HttpStatusCode.NotFound, Error.NotFound("NOT_FOUND", ex.Message)),
            InvalidOperationException ex =>
                (HttpStatusCode.Conflict, Error.Conflict("CONFLICT", ex.Message)),
            _ =>
                (HttpStatusCode.InternalServerError, Error.Internal("INTERNAL_ERROR", "An unexpected error occurred"))
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = new
            {
                code = error.Code,
                message = error.Message,
                requestId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow.ToString("O")
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
