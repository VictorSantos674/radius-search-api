using FluentValidation;
using RadiusSearch.Application.Common;
using RadiusSearch.Domain.Repositories;
using RadiusSearch.Domain.Services;
using RadiusSearch.Domain.ValueObjects;

namespace RadiusSearch.Application.UseCases.FindEquipmentByRadius;

public sealed class FindEquipmentByRadiusUseCase
{
    private readonly IEquipmentRepository _repository;
    private readonly FindEquipmentByRadiusValidator _validator;

    public FindEquipmentByRadiusUseCase(
        IEquipmentRepository repository,
        FindEquipmentByRadiusValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<PagedResult<FindEquipmentByRadiusResult>> ExecuteAsync(
        FindEquipmentByRadiusQuery query,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(query, cancellationToken);

        var origin = new Coordinate(query.Latitude, query.Longitude);

        var allMatches = _repository
            .GetAll()
            .Where(equipment => equipment.IsAvailable())
            .Select(equipment => (
                Equipment: equipment,
                Distance: HaversineService.CalculateDistanceInMeters(origin, equipment.Location)
            ))
            .Where(result => result.Distance <= query.Radius)
            .OrderBy(result => result.Distance)
            .Select(result => new FindEquipmentByRadiusResult(
                Id: result.Equipment.Id,
                Nome: result.Equipment.Name,
                Latitude: result.Equipment.Location.Latitude,
                Longitude: result.Equipment.Location.Longitude,
                Radius: Math.Round(result.Distance, 2)))
            .ToList();

        var paged = allMatches
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PagedResult<FindEquipmentByRadiusResult>
        {
            Items = paged,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = allMatches.Count
        };
    }
}
