using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using TaxCalculator.Model.Interfaces.Caching;

namespace TaxCalculator.Caching
{
    public class InMemoryCachingService : ICachingService
    {
        private readonly IMemoryCache _memoryCache;

        [JsonConstructor]
        public InMemoryCachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out T value) ? value : default);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await Task.CompletedTask;
        }
    }

}
