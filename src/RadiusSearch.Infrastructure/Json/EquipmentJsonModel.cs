using System.Text.Json.Serialization;

namespace RadiusSearch.Infrastructure.Json;

internal sealed record EquipmentJsonModel
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("geometry")]
    public List<GeometryPoint> Geometry { get; init; } = [];
}

internal sealed record GeometryPoint
{
    [JsonPropertyName("x")]
    public double? X { get; init; }

    [JsonPropertyName("y")]
    public double? Y { get; init; }
}
