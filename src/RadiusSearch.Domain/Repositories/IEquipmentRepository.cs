using RadiusSearch.Domain.Entities;
using RadiusSearch.Domain.ValueObjects;

namespace RadiusSearch.Domain.Repositories;

public interface IEquipmentRepository
{
    IEnumerable<(Equipment Equipment, double DistanceMeters)> FindWithinRadius(
        Coordinate origin,
        int radiusMeters);
}
