using FluentAssertions;
using FluentValidation;
using NSubstitute;
using RadiusSearch.Application.UseCases.FindEquipmentByRadius;
using RadiusSearch.Domain.Entities;
using RadiusSearch.Domain.Enums;
using RadiusSearch.Domain.Repositories;
using RadiusSearch.Domain.ValueObjects;

namespace RadiusSearch.Tests.Application;

public class FindEquipmentByRadiusUseCaseTests
{
    private readonly IEquipmentRepository _repository = Substitute.For<IEquipmentRepository>();
    private readonly FindEquipmentByRadiusValidator _validator = new();
    private readonly FindEquipmentByRadiusUseCase _sut;

    public FindEquipmentByRadiusUseCaseTests()
    {
        _sut = new FindEquipmentByRadiusUseCase(_repository, _validator);
    }

    [Fact]
    public async Task ExecuteAsync_ValidQuery_ReturnsPagedResults()
    {
        var coord = new Coordinate(-22.91016, -43.18298);

        _repository
            .GetAll()
            .Returns([
                new Equipment(1, "CTO-001", EquipmentStatus.Active, coord),
                new Equipment(2, "CTO-002", EquipmentStatus.Reserved, coord),
                new Equipment(3, "CTO-003", EquipmentStatus.Planned, coord),
                new Equipment(4, "CTO-004", EquipmentStatus.Active, new Coordinate(-10.00000, -140.00000))
            ]);

        var query = CreateQuery(-22.91016, -43.18298, 200);

        var result = await _sut.ExecuteAsync(query);

        result.Items.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidLatitude_ThrowsValidationException()
    {
        var query = new FindEquipmentByRadiusQuery(
            Latitude: 0.0,
            Longitude: -43.18298,
            Radius: 200,
            RawLatitude: "0.0",
            RawLongitude: "-43.18298");

        var act = async () => await _sut.ExecuteAsync(query);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_RadiusOutOfRange_ThrowsValidationException()
    {
        var query = CreateQuery(-22.91016, -43.18298, 5);

        var act = async () => await _sut.ExecuteAsync(query);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_PaginationWorks()
    {
        var coord = new Coordinate(-22.91016, -43.18298);
        var fixtures = Enumerable.Range(1, 25)
            .Select(index => new Equipment(index, $"CTO-{index:000}", EquipmentStatus.Active, coord))
            .ToList();

        _repository
            .GetAll()
            .Returns(fixtures);

        var page1 = await _sut.ExecuteAsync(CreateQuery(-22.91016, -43.18298, 1000, page: 1, pageSize: 20));
        var page2 = await _sut.ExecuteAsync(CreateQuery(-22.91016, -43.18298, 1000, page: 2, pageSize: 20));

        page1.Items.Should().HaveCount(20);
        page2.Items.Should().HaveCount(5);
        page1.TotalPages.Should().Be(2);
    }

    private static FindEquipmentByRadiusQuery CreateQuery(
        double latitude,
        double longitude,
        int radius,
        int page = 1,
        int pageSize = 20)
    {
        return new FindEquipmentByRadiusQuery(
            Latitude: latitude,
            Longitude: longitude,
            Radius: radius,
            Page: page,
            PageSize: pageSize,
            RawLatitude: latitude.ToString("F5", System.Globalization.CultureInfo.InvariantCulture),
            RawLongitude: longitude.ToString("F5", System.Globalization.CultureInfo.InvariantCulture));
    }
}
