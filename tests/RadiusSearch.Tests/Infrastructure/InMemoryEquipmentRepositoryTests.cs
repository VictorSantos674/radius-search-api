using FluentAssertions;
using RadiusSearch.Domain.ValueObjects;
using RadiusSearch.Infrastructure.Repositories;

namespace RadiusSearch.Tests.Infrastructure;

public class InMemoryEquipmentRepositoryTests
{
    private const string DatasetPath = "data/dataset_v2.json";

    private readonly InMemoryEquipmentRepository _sut = new(DatasetPath);

    [Fact]
    public void FindWithinRadius_SmallRadius_ReturnsOnlyActiveAndReserved()
    {
        var origin = new Coordinate(-22.910159, -43.182978);

        var results = _sut.FindWithinRadius(origin, radiusMeters: 1000).ToList();

        results.Should().OnlyContain(result => result.Equipment.IsAvailable());
    }

    [Fact]
    public void FindWithinRadius_OrderedByDistance()
    {
        var origin = new Coordinate(-22.910159, -43.182978);

        var distances = _sut.FindWithinRadius(origin, radiusMeters: 5000)
            .Select(result => result.DistanceMeters)
            .ToList();

        distances.Should().BeInAscendingOrder();
    }

    [Fact]
    public void FindWithinRadius_ZeroResults_WhenNothingIsNear()
    {
        var middleOfOcean = new Coordinate(-10.00000, -140.00000);

        var results = _sut.FindWithinRadius(middleOfOcean, radiusMeters: 1000);

        results.Should().BeEmpty();
    }
}
