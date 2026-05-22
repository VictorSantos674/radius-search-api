using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RadiusSearch.Api.Models;
using RadiusSearch.Application.UseCases.FindEquipmentByRadius;

namespace RadiusSearch.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class FeasibilityController : ControllerBase
{
    private readonly FindEquipmentByRadiusUseCase _useCase;

    public FeasibilityController(FindEquipmentByRadiusUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet("feasibility")]
    [Produces("application/json")]
    public async Task<IActionResult> Get(
        [FromQuery] string? latitude,
        [FromQuery] string? longitude,
        [FromQuery] int? radius,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (latitude is null)
        {
            return MissingParam("latitude");
        }

        if (longitude is null)
        {
            return MissingParam("longitude");
        }

        if (radius is null)
        {
            return MissingParam("radius");
        }

        if (!double.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
        {
            return InvalidParam("latitude must be a valid float");
        }

        if (!double.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
        {
            return InvalidParam("longitude must be a valid float");
        }

        var query = new FindEquipmentByRadiusQuery(
            lat,
            lon,
            radius.Value,
            page,
            pageSize,
            latitude,
            longitude);

        var result = await _useCase.ExecuteAsync(query, cancellationToken);

        return Ok(result.Items);
    }

    private static BadRequestObjectResult InvalidParam(string message)
    {
        return new BadRequestObjectResult(new ErrorResponse(
            Code: "400",
            Reason: "invalid field",
            Message: message,
            Status: "bad request",
            Timestamp: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")));
    }

    private static BadRequestObjectResult MissingParam(string param)
    {
        return new BadRequestObjectResult(new ErrorResponse(
            Code: "400",
            Reason: "empty field",
            Message: $"{param} is mandatory",
            Status: "bad request",
            Timestamp: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")));
    }
}
