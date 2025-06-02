namespace Application
{
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
            });

            services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));
        }
    }
}
