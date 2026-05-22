using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiusSearch.Domain.Repositories;
using RadiusSearch.Infrastructure.Repositories;

namespace RadiusSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var datasetPath = configuration["Dataset:Path"]
            ?? throw new InvalidOperationException("Dataset:Path not configured.");

        var resolvedDatasetPath = Path.IsPathRooted(datasetPath)
            ? datasetPath
            : Path.Combine(AppContext.BaseDirectory, datasetPath);

        services.AddSingleton<IEquipmentRepository>(
            _ => new InMemoryEquipmentRepository(resolvedDatasetPath));

        return services;
    }
}
