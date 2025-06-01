namespace Application
{
    using Application.UseCases.Users.Queries;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
            });
        }
    }
}
