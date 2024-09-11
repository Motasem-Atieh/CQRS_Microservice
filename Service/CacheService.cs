using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public class CacheService<T>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService<T>> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService<T>> logger)
    {
        _cache = cache;
        _logger = logger;
    }


    public async Task<IEnumerable<T>> GetCollectionFromCacheAsync(string cacheKey)
    {
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation($"Cache hit for key: {cacheKey}");
            return JsonSerializer.Deserialize<IEnumerable<T>>(cachedData);
        }

        _logger.LogInformation($"Cache miss for key: {cacheKey}");
        return null;
    }



    public async Task<T> GetFromCacheAsync(string cacheKey)
    {
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation($"Cache hit for key: {cacheKey}");
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        _logger.LogInformation($"Cache miss for key: {cacheKey}");
        return default;
    }

    //for single product
    public async Task SetCacheAsync(string cacheKey, T data, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(10),
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromHours(1)
        };

        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
        _logger.LogInformation($"Data cached with key: {cacheKey}");
    }
    //for all products
    public async Task SetCacheAsync(string cacheKey, IEnumerable<T> data, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(10),
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromHours(1)
        };

        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
        _logger.LogInformation($"Data collection cached with key: {cacheKey}");
    }


    public async Task RemoveCacheAsync(string cacheKey)
    {
        await _cache.RemoveAsync(cacheKey);
        _logger.LogInformation($"Cache removed with key: {cacheKey}");
    }

    // Populate cache with collection of items
    public async Task PopulateCacheAsync(IEnumerable<T> allData, Func<T, string> getCacheKey)
    {
        foreach (var item in allData)
        {
            var cacheKey = getCacheKey(item);
            await SetCacheAsync(cacheKey, item);
        }
        _logger.LogInformation("Cache populated with data from database.");
    }
}
