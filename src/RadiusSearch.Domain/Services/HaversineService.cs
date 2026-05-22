using RadiusSearch.Domain.ValueObjects;

namespace RadiusSearch.Domain.Services;

public static class HaversineService
{
    private const double EarthRadiusMeters = 6_371_000;

    public static double CalculateDistanceInMeters(Coordinate origin, Coordinate destination)
    {
        var lat1 = ToRadians(origin.Latitude);
        var lat2 = ToRadians(destination.Latitude);
        var dLat = ToRadians(destination.Latitude - origin.Latitude);
        var dLon = ToRadians(destination.Longitude - origin.Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1) * Math.Cos(lat2)
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
