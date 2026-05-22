using RadiusSearch.Api.Middleware;
using RadiusSearch.Api.Models;
using RadiusSearch.Application;
using RadiusSearch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var message = context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .FirstOrDefault() ?? "invalid request";

            var response = new ErrorResponse(
                Code: "400",
                Reason: "validation error",
                Message: message,
                Status: "bad request",
                Timestamp: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
        };
    });
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program
{
}
