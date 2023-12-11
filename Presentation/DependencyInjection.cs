using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Presentation;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationLayerServices(this IServiceCollection services)
    {
        services.AddSingleton<ReceiveInject>();
        return services;
    }
}
