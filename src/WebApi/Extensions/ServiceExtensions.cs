using LearningManagementSystem.Infrastructure.Identity.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigOptions(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<SecurityOptions>()
            .Bind(config.GetSection(SecurityOptions.Security))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static void AddApiVersioningExtension(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });
    }
}
