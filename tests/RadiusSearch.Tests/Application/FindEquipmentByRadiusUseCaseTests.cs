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
            .FindWithinRadius(Arg.Any<Coordinate>(), Arg.Any<int>())
            .Returns([
                (new Equipment(1, "CTO-001", EquipmentStatus.Active, coord), 50.0),
                (new Equipment(2, "CTO-002", EquipmentStatus.Reserved, coord), 120.0)
            ]);

        var query = new FindEquipmentByRadiusQuery(-22.91016, -43.18298, 200);

        var result = await _sut.ExecuteAsync(query);

        result.Items.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidLatitude_ThrowsValidationException()
    {
        var query = new FindEquipmentByRadiusQuery(0.0, -43.18298, 200);

        var act = async () => await _sut.ExecuteAsync(query);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_RadiusOutOfRange_ThrowsValidationException()
    {
        var query = new FindEquipmentByRadiusQuery(-22.91016, -43.18298, Radius: 5);

        var act = async () => await _sut.ExecuteAsync(query);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_PaginationWorks()
    {
        var coord = new Coordinate(-22.91016, -43.18298);
        var fixtures = Enumerable.Range(1, 25)
            .Select(index => (
                Equipment: new Equipment(index, $"CTO-{index:000}", EquipmentStatus.Active, coord),
                DistanceMeters: (double)(index * 10)))
            .ToList();

        _repository
            .FindWithinRadius(Arg.Any<Coordinate>(), Arg.Any<int>())
            .Returns(fixtures);

        var page1 = await _sut.ExecuteAsync(new FindEquipmentByRadiusQuery(-22.91016, -43.18298, 1000, Page: 1, PageSize: 20));
        var page2 = await _sut.ExecuteAsync(new FindEquipmentByRadiusQuery(-22.91016, -43.18298, 1000, Page: 2, PageSize: 20));

        page1.Items.Should().HaveCount(20);
        page2.Items.Should().HaveCount(5);
        page1.TotalPages.Should().Be(2);
    }
}
