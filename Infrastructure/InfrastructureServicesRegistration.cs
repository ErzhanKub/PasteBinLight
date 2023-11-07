using Application.Shared;
using Domain.IServices;
using Domain.Repositories;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Infrastructure.Services;
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
        services.AddScoped<IRecordRepository, RecordRepository>();
        services.AddScoped<IRecordCloudService, RecordCloudService>();
        services.AddScoped<IQRCodeGeneratorService, QRCodeGeneratorService>();

        return services;
    }
}
