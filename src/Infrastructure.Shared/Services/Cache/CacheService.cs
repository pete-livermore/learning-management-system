using System.Text.Json;
using Application.Common.Interfaces.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Shared.Services.Cache;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetValueAsync<T>(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedData))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (JsonException)
        {
            _logger.LogError($"There was an error deserializing cached data with key: {key}");
            return default;
        }
    }

    public async Task SetValueAsync<T>(string key, T data)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), cacheOptions);
    }
}
