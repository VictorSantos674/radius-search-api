using System.Diagnostics;

namespace RadiusSearch.Api.Middleware;

public sealed class RequestHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public RequestHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            context.Response.Headers["X-Request-Id"] = requestId;
            context.Response.Headers["X-Response-Time"] = $"{stopwatch.ElapsedMilliseconds}ms";

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
