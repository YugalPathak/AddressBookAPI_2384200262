using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace BusinessLayer.Configurations
{
    /// <summary>
    /// Configuration class for establishing a connection to the Redis cache.
    /// </summary>
    public class RedisConfig
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Gets the Redis connection multiplexer instance.
        /// </summary>
        public ConnectionMultiplexer Connection { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConfig"/> class.
        /// Establishes a connection to the Redis server using the connection string from the configuration.
        /// </summary>
        /// <param name="configuration">Application configuration containing Redis connection settings.</param>
        public RedisConfig(IConfiguration configuration)
        {
            _configuration = configuration;
            Connection = ConnectionMultiplexer.Connect(_configuration["Redis:ConnectionString"]);
        }
    }
}