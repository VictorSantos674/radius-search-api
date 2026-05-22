using System.Text.Json;
using RadiusSearch.Domain.Entities;
using RadiusSearch.Domain.Enums;
using RadiusSearch.Domain.Repositories;
using RadiusSearch.Domain.Services;
using RadiusSearch.Domain.ValueObjects;
using RadiusSearch.Infrastructure.Json;

namespace RadiusSearch.Infrastructure.Repositories;

public sealed class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly IReadOnlyList<Equipment> _equipment;

    public InMemoryEquipmentRepository(string datasetPath)
    {
        _equipment = LoadDataset(datasetPath);
    }

    public IEnumerable<(Equipment Equipment, double DistanceMeters)> FindWithinRadius(
        Coordinate origin,
        int radiusMeters)
    {
        return _equipment
            .Where(equipment => equipment.IsAvailable())
            .Select(equipment => (
                Equipment: equipment,
                DistanceMeters: HaversineService.CalculateDistanceInMeters(origin, equipment.Location)
            ))
            .Where(result => result.DistanceMeters <= radiusMeters)
            .OrderBy(result => result.DistanceMeters);
    }

    private static IReadOnlyList<Equipment> LoadDataset(string path)
    {
        using var stream = File.OpenRead(path);

        var models = JsonSerializer.Deserialize<List<EquipmentJsonModel>>(stream)
            ?? throw new InvalidOperationException("Dataset is null or empty.");

        var result = new List<Equipment>(models.Count);

        foreach (var model in models)
        {
            if (!TryParseStatus(model.Status, out var status))
            {
                continue;
            }

            if (model.Geometry is not [var point, ..])
            {
                continue;
            }

            if (point.Y is not { } latitude || point.X is not { } longitude)
            {
                continue;
            }

            if (IsInvalidCoordinate(latitude, longitude))
            {
                continue;
            }

            var coordinate = new Coordinate(latitude, longitude);
            result.Add(new Equipment(model.Id, model.Name, status, coordinate));
        }

        return result.AsReadOnly();
    }

    private static bool TryParseStatus(string raw, out EquipmentStatus status) =>
        Enum.TryParse(raw, ignoreCase: true, out status);

    private static bool IsInvalidCoordinate(double latitude, double longitude) =>
        latitude is < -90 or > 90 || longitude is < -180 or > 180;
}
