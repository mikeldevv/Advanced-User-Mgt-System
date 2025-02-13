using Microsoft.Extensions.Configuration;
using UMS.Contracts;
using StackExchange.Redis;

namespace UMS.Features;

public class RedisCache : ICache
{
    private readonly ConnectionMultiplexer _redisConnection;

    public RedisCache(IConfiguration configuration)
    {
        string? redisConnectionString = configuration.GetConnectionString(nameof(RedisCache));
        _redisConnection = ConnectionMultiplexer.Connect(redisConnectionString!);
    }


    public async Task<(bool Hit, TimeSpan? ExpiresIn)> Exists(string key)
    {
        IDatabase database = _redisConnection.GetDatabase();
        TimeSpan? ttl = await database.KeyTimeToLiveAsync(key);
        
        return (ttl.HasValue, ttl);
    }

    public async Task<(string Value, TimeSpan ExpiresIn)?> Read(string key)
    {
        IDatabase database = _redisConnection.GetDatabase();
        RedisValue value = await database.StringGetAsync(key);
        TimeSpan? ttl = await database.KeyTimeToLiveAsync(key);
        
        return (value, ttl ?? TimeSpan.Zero);
    }

    public async Task Delete(string key)
    {
        IDatabase database = _redisConnection.GetDatabase();
        await database.KeyDeleteAsync(key);
    }

    public async Task Write(string key, string value, bool preserveTtlIfKeyIsExpiring = false, TimeSpan? ttl = null)
    {
        IDatabase database = _redisConnection.GetDatabase();
        if (preserveTtlIfKeyIsExpiring)
        {
            TimeSpan? existingTtl = await database.KeyTimeToLiveAsync(key);
            if (existingTtl.HasValue && existingTtl.Value.TotalMilliseconds > 0)
            {
                ttl = existingTtl;
            }
        }

        await database.StringSetAsync(key, value, ttl);
    }
}
