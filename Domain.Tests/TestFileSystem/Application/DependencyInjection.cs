using CleanArchitectureWorkshop.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureWorkshop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddScoped<ITeamsUseCase, TeamsUseCase>();

        return services;
    }
}
