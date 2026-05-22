using System.Net;
using System.Text.Json;
using FluentValidation;
using RadiusSearch.Api.Models;

namespace RadiusSearch.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                "Validation failed for {Path}: {Errors}",
                context.Request.Path,
                string.Join("; ", ex.Errors.Select(error => error.ErrorMessage)));

            await WriteErrorAsync(
                context,
                HttpStatusCode.BadRequest,
                code: "400",
                reason: "validation error",
                message: ex.Errors.First().ErrorMessage,
                status: "bad request");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                code: "500",
                reason: "internal server error",
                message: "general fail",
                status: "internal server error");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string code,
        string reason,
        string message,
        string status)
    {
        var response = new ErrorResponse(
            Code: code,
            Reason: reason,
            Message: message,
            Status: status,
            Timestamp: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
