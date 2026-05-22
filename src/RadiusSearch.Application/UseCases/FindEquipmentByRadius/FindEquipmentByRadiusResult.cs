namespace RadiusSearch.Application.UseCases.FindEquipmentByRadius;

public sealed record FindEquipmentByRadiusResult(
    int Id,
    string Nome,
    double Latitude,
    double Longitude,
    double Radius);
