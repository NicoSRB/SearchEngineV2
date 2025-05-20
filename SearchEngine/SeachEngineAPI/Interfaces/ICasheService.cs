using Shared.Model;


namespace SeachEngineAPI.Interfaces
{
    public interface ICacheService
    {
        Task<string?> GetCachedResultAsync(string key);
        Task SetCachedResultAsync(string key, string? result, TimeSpan expiration);
    }
}
