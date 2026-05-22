using Microsoft.Extensions.DependencyInjection;
using RadiusSearch.Application.UseCases.FindEquipmentByRadius;

namespace RadiusSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<FindEquipmentByRadiusUseCase>();
        services.AddScoped<FindEquipmentByRadiusValidator>();

        return services;
    }
}
