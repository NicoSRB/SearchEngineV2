using Microsoft.OpenApi.Services;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace SeachEngineAPI.Services
{
    public class SearchCacheService
    {
        private readonly IDatabase _cache;

        public SearchCacheService(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }

        public async Task<SearchResult?> GetCachedResultAsync(string key)
        {
            var cached = await _cache.StringGetAsync(key);
            if (cached.IsNullOrEmpty) return null;

            // Convert RedisValue to string before deserialization
            return JsonConvert.DeserializeObject<SearchResult>(cached.ToString());
        }

        public async Task SetCachedResultAsync(string key, SearchResult result, TimeSpan expiration)
        {
            // Serialize using Newtonsoft.Json
            var json = JsonConvert.SerializeObject(result);
            await _cache.StringSetAsync(key, json, expiration);
        }
    }
}
