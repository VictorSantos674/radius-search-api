using Microsoft.AspNetCore.Mvc;
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
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
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

        var query = new FindEquipmentByRadiusQuery(
            latitude.Value,
            longitude.Value,
            radius.Value,
            page,
            pageSize);

        var result = await _useCase.ExecuteAsync(query, cancellationToken);

        return Ok(result.Items);
    }

    private static BadRequestObjectResult MissingParam(string param)
    {
        return new BadRequestObjectResult(new
        {
            code = "400",
            reason = "empty field",
            message = $"{param} is mandatory",
            status = "bad request",
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        });
    }
}
