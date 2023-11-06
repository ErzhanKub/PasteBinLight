using Application.Shared;
using Domain.Repositories;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastruct(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseSqlServer(configuration.GetConnectionString("Default") ??
                "Server=.; Database=PosteBin; Trusted_Connection=SSPI; Encrypt=Optional");
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasteRepository, PosteRepository>();

        return services;
    }
}
