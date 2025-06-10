using Application.Common.Behaviours;
using Application.UseCases.Uploads.Helpers;
using Application.UseCases.Uploads.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));
            services.AddTransient<IFileValidator, FileValidator>();
        }
    }
}
