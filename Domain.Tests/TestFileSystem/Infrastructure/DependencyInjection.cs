using CleanArchitectureWorkshop.Application.Repositories;
using CleanArchitectureWorkshop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureWorkshop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=CleanArchitectureWorkshop.Database;Integrated Security=true;"));

        services.AddScoped<ITeamsRepository, TeamsRepository>();

        return services;
    }
}
