using FluentAssertions;
using RadiusSearch.Infrastructure.Repositories;

namespace RadiusSearch.Tests.Infrastructure;

public class InMemoryEquipmentRepositoryTests
{
    private const string DatasetPath = "data/dataset_v2.json";

    private readonly InMemoryEquipmentRepository _sut = new(DatasetPath);

    [Fact]
    public void GetAll_LoadsDataset()
    {
        var results = _sut.GetAll().ToList();

        results.Should().NotBeEmpty();
    }

    [Fact]
    public void GetAll_LoadsValidCoordinates()
    {
        var results = _sut.GetAll().ToList();

        results.Should().OnlyContain(equipment =>
            equipment.Location.Latitude >= -90
            && equipment.Location.Latitude <= 90
            && equipment.Location.Longitude >= -180
            && equipment.Location.Longitude <= 180);
    }

    [Fact]
    public void GetAll_KeepsUnavailableEquipmentForApplicationFiltering()
    {
        var results = _sut.GetAll().ToList();

        results.Should().Contain(equipment => !equipment.IsAvailable());
    }
}
