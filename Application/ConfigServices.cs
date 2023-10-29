﻿using Application.Features.Users.Create;
using Application.Features.Users.Login;
using Application.Pipelines;
using Microsoft.AspNetCore.Hosting;
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

            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);

            services.AddTransient(
               typeof(IPipelineBehavior<,>),
               typeof(ValidationPipeline<,>));


            return services;
        }
    }
}