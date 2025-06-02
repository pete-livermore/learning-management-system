using Application.Common.Interfaces.Cache;
using Infrastructure.Shared.Configuration.Redis;
using Infrastructure.Shared.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared;

public static class ServiceRegistration
{
    private static void AddCacheInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var redisConfig = config.GetSection(RedisOptions.Redis).Get<RedisOptions>();
        if (redisConfig is null)
        {
            throw new InvalidOperationException(
                $"{RedisOptions.Redis} config is missing or incomplete in appsettings.json."
            );
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig.Configuration;
            options.InstanceName = redisConfig.InstanceName;
        });

        services.AddScoped<ICacheService, CacheService>();
    }

    public static void AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddCacheInfrastructure(config);
    }
}
