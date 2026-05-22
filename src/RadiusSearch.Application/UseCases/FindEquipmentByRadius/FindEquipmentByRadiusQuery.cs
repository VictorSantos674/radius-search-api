namespace RadiusSearch.Application.UseCases.FindEquipmentByRadius;

public sealed record FindEquipmentByRadiusQuery(
    double Latitude,
    double Longitude,
    int Radius,
    int Page = 1,
    int PageSize = 20,
    string RawLatitude = "",
    string RawLongitude = "");
