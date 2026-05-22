using RadiusSearch.Domain.Enums;
using RadiusSearch.Domain.ValueObjects;

namespace RadiusSearch.Domain.Entities;

public sealed class Equipment
{
    public int Id { get; }
    public string Name { get; }
    public EquipmentStatus Status { get; }
    public Coordinate Location { get; }

    public Equipment(int id, string name, EquipmentStatus status, Coordinate location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
        Status = status;
        Location = location;
    }

    public bool IsAvailable() =>
        Status is EquipmentStatus.Active or EquipmentStatus.Reserved;
}
