using Application.Features.Users.Login;
using Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ConfigServices
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(
                       Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);

            services.AddTransient(
               typeof(IPipelineBehavior<,>),
               typeof(ValidationPipeline<,>));


            return services;
        }
    }
}