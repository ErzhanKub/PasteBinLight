// Importing the required libraries
using Application.Features.Users.Create;
using Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// Namespace for the application
namespace Application
{
    // Static class for registering application services
    public static class ApplicationServicesRegistration
    {
        // Extension method for IServiceCollection
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add logging services
            services.AddLogging();

            // Add MediatR services and register services from the current assembly
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(
                   Assembly.GetExecutingAssembly()));

            // Add validators from the assembly of CreateUserCommandValidator
            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

            // Add transient service for the validation pipeline
            services.AddTransient(
               typeof(IPipelineBehavior<,>),
               typeof(RequestValidationPipeline<,>));

            // Return the service collection
            return services;
        }
    }
}
