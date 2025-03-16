using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Service for handling Redis cache operations.
    /// Provides methods to set and get cached data asynchronously.
    /// </summary>
    public class RedisCacheService
    {
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
        /// </summary>
        /// <param name="cache">IDistributedCache instance for interacting with Redis.</param>
        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Stores data in Redis cache with an expiration time.
        /// </summary>
        /// <typeparam name="T">Type of data to cache.</typeparam>
        /// <param name="key">Unique key to identify cached data.</param>
        /// <param name="value">Data to be cached.</param>
        /// <param name="expirationTime">Time duration after which cache expires.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expirationTime)
        {
            var serializedData = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            });
        }

        /// <summary>
        /// Retrieves data from Redis cache.
        /// </summary>
        /// <typeparam name="T">Type of data to retrieve.</typeparam>
        /// <param name="key">Unique key to fetch cached data.</param>
        /// <returns>Cached data if found, otherwise default value of T.</returns>
        public async Task<T> GetCacheAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            return data == null ? default : JsonConvert.DeserializeObject<T>(data);
        }
    }
}