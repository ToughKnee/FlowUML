using Domain.Mediators;
using Infrastructure.Antlr;
using Infrastructure.Mediators;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, AntlrMediator>();
        services.AddSingleton<CSharpVisitor>();
        services.AddSingleton<ANTLRService>();
        return services;
    }
}
