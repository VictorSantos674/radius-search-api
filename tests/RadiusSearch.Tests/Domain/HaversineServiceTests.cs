using FluentAssertions;
using RadiusSearch.Domain.Entities;
using RadiusSearch.Domain.Enums;
using RadiusSearch.Domain.Services;
using RadiusSearch.Domain.ValueObjects;

namespace RadiusSearch.Tests.Domain;

public class HaversineServiceTests
{
    [Fact]
    public void CalculateDistance_SamePoint_ReturnsZero()
    {
        var point = new Coordinate(-23.55646, -46.63565);

        var result = HaversineService.CalculateDistanceInMeters(point, point);

        result.Should().BeApproximately(0, precision: 0.001);
    }

    [Fact]
    public void CalculateDistance_KnownPoints_ReturnsExpectedMeters()
    {
        var paulista = new Coordinate(-23.56170, -46.65580);
        var ibirapuera = new Coordinate(-23.58745, -46.65780);

        var result = HaversineService.CalculateDistanceInMeters(paulista, ibirapuera);

        result.Should().BeInRange(2_800, 3_000);
    }

    [Fact]
    public void Coordinate_InvalidLatitude_Throws()
    {
        var act = () => new Coordinate(91, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Equipment_IsAvailable_ReturnsTrueForActiveAndReserved()
    {
        var coord = new Coordinate(-23.55646, -46.63565);
        var active = new Equipment(1, "CTO-001", EquipmentStatus.Active, coord);
        var reserved = new Equipment(2, "CTO-002", EquipmentStatus.Reserved, coord);
        var planned = new Equipment(3, "CTO-003", EquipmentStatus.Planned, coord);

        active.IsAvailable().Should().BeTrue();
        reserved.IsAvailable().Should().BeTrue();
        planned.IsAvailable().Should().BeFalse();
    }
}
