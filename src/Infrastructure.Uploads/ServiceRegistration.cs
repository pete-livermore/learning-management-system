using Application.UseCases.Uploads.Interfaces;
using Infrastructure.Uploads.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Uploads;

public static class ServiceRegistration
{
    public static void AddUploadsInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IUploadsService, UploadsService>();
    }
}
