using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.Models
{
    public enum CacheKeys
    {
        TeacherName,
    }
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task SetCacheAsync (CacheKeys key, string value, TimeSpan expiration)
        {
            _cache.Set(key.ToString(), value, expiration);
        }

        public async Task<string?> GetCacheValue(CacheKeys key)
        {
            _cache.TryGetValue(key.ToString(), out string? value);
            return value;
        }
    }
}
