using Microsoft.OpenApi.Services;
using Newtonsoft.Json;
using SeachEngineAPI.Interfaces;
using StackExchange.Redis;

namespace SeachEngineAPI.Services
{
    public class SearchCacheService : ICacheService
    {
       
        private readonly IDatabase _db;

        public SearchCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<string?> GetCachedResultAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task SetCachedResultAsync(string key, string? result, TimeSpan expiration)
        {
            await _db.StringSetAsync(key, result, expiration);
        }
    }
}
