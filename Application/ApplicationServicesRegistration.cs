using Application.Features.Users.Create;
using Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(
                       Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

            services.AddTransient(
               typeof(IPipelineBehavior<,>),
               typeof(ValidationPipeline<,>));


            return services;
        }
    }
}