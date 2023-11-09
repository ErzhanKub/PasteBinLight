using Application.Shared;
using Domain.IServices;
using Domain.Repositories;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.TelegramService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    // Static class for registering infrastructure services
    public static class InfrastructureServicesRegistration
    {
        // Extension method for IServiceCollection
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext to the services
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("Default") ??
                    "Server=.; Database=PosteBin; Trusted_Connection=SSPI; Encrypt=Optional");
            });

            // Register repositories and services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRecordRepository, RecordRepository>();
            services.AddScoped<IRecordCloudService, CloudService>();
            services.AddScoped<IQRCodeGeneratorService, QRCodeGeneratorService>();
            services.AddScoped<ITelegramService, TelegramService>();

            return services;
        }
    }
}
