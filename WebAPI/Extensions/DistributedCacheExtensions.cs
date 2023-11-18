using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WebAPI.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task SetRecordAsync<T>(this IDistributedCache distributedCache,
        string recordId, 
        T data, 
        TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(60);
        options.SlidingExpiration = unusedExpireTime;

        var jsonData = JsonSerializer.Serialize<T>(data);
        await distributedCache.SetStringAsync(recordId, jsonData, options);
    }

    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache distributedCache, string recordId)
    {
        var jsonData = await distributedCache.GetStringAsync(recordId);

        if (jsonData is null)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>(jsonData)!;
    }
}