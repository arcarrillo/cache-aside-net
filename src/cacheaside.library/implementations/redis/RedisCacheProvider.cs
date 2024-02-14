using cacheaside.library.interfaces;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace cacheaside.library.implementations.redis;

public class RedisCacheProvider : ICacheProvider
{
    ConnectionMultiplexer? _connection;
    readonly RedisConnectionConfiguration _configuration;

    public RedisCacheProvider(RedisConnectionConfiguration configuration)
    {
        _configuration = configuration;
    }

    ConnectionMultiplexer GetConnection()
    {
        if (_connection == null)
        {
            _connection = ConnectionMultiplexer.Connect(_configuration.ConnectionString, config =>
            {
                config.ConnectTimeout = _configuration.ConnectionTimeout;
            });
        }
        return _connection;
    }

    IDatabase? GetDatabase() => GetConnection()?.GetDatabase();
    IServer? GetServer() => GetConnection().GetServers().LastOrDefault();

    public Task<bool> Exists(string key) => GetDatabase()?.KeyExistsAsync(new RedisKey(key)) ?? Task.FromResult(false);

    public async Task<T?> GetValueAsync<T>(string key) where T : class
    {
        var database = GetDatabase();
        if (database != null)
        {
            var data = await database.StringGetAsync(new RedisKey(key));
            if (data.HasValue && !data.IsNullOrEmpty)
                return JsonSerializer.Deserialize<T>(data.ToString());
        }
        
        return null;
    }

    public async Task<T?> GetValueOrInitializeAsync<T>(string key, Func<Task<T>> functionToObtain, TimeSpan duration) where T : class
    {
        var value = await GetValueAsync<T>(key);

        if (value == null)
        {
            value = await functionToObtain.Invoke();
            if (value != null)
                await SetValueAsync(key, value, duration);
        }

        return value;
    }

    public bool RemoveByPattern(string pattern)
    {
        var keys = GetServer()?.Keys(pattern:new RedisValue(pattern));
        bool result = true;
        if (keys != null)
        {
            foreach (var key in keys)
            {
                if (!GetDatabase()?.KeyDelete(key) ?? false)
                    result = false;
            }
        }

        return result;
    }

    public Task<bool> SetValueAsync<T>(string key, T value, TimeSpan duration) where T : class =>
        GetDatabase()?.StringSetAsync(new RedisKey(key), new RedisValue(JsonSerializer.Serialize(value)), duration) ??
            Task.FromResult(false);

}
