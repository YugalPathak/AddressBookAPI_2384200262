using NUnit.Framework;
using BusinessLayer.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace AddressBookTests.Service
{
    [TestFixture]
    public class RedisCacheServiceTests
    {
        private RedisCacheService _redisCacheService;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            // ✅ Register Redis Distributed Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // Ensure Redis is running
                options.InstanceName = "RedisCacheInstance";
            });

            var serviceProvider = services.BuildServiceProvider();
            var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();

            // ✅ Initialize RedisCacheService with IDistributedCache
            _redisCacheService = new RedisCacheService(distributedCache);
        }

        [TearDown]
        public async Task TearDown()
        {
            // ✅ Cleanup after tests
            await _redisCacheService.SetCacheAsync("testKey", string.Empty, TimeSpan.Zero);
        }

        [Test]
        public async Task SetCacheAsync_Should_Store_Value_In_Cache()
        {
            // Arrange
            string key = "testKey";
            string value = "Hello, Redis!";
            TimeSpan expiration = TimeSpan.FromMinutes(5);

            // Act
            await _redisCacheService.SetCacheAsync(key, value, expiration);
            var cachedValue = await _redisCacheService.GetCacheAsync<string>(key);

            // Assert ✅ Ensure value is stored correctly
            Assert.IsNotNull(cachedValue, "Cache retrieval failed.");
            Assert.AreEqual(value, cachedValue, "Cached value does not match.");
        }

        [Test]
        public async Task GetCacheAsync_Should_Return_Default_If_Key_Not_Found()
        {
            // Arrange
            string key = "nonExistingKey";

            // Act
            var cachedValue = await _redisCacheService.GetCacheAsync<string>(key);

            // Assert ✅ Ensure it returns default (null for reference types)
            Assert.IsNull(cachedValue, "Non-existing key should return null.");
        }
    }
}