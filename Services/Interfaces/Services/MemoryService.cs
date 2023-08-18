using Polly.Retry;
using Polly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class MemoryService : IMemoryService
    {
        private readonly IConnectionMultiplexer connection;

        public MemoryService(IConnectionMultiplexer connectionMultiplexer)
        {
            connection = connectionMultiplexer;

        }

        //   try connection for multiple times
        private AsyncRetryPolicy CreateRetryPolicy()
        {
            return Policy.Handle<RedisConnectionException>() // Specify the exception type to handle
                         .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            //var retryPolicy = CreateRetryPolicy();

            //return await retryPolicy.ExecuteAsync(async () =>
            //{
                var database = connection.GetDatabase();

                var value = await database.StringGetAsync(key);
                return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
            //});
            }
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var database = connection.GetDatabase();

            var serializedValue = JsonSerializer.Serialize(value);
            await database.StringSetAsync(key, serializedValue, expiration);
        }
        public async Task RemoveAsync(string key)
        {
            var database = connection.GetDatabase();

            await database.KeyDeleteAsync(key);
        }

        public async Task<IEnumerable<T>> RangeSortedSet<T>(string sortedKey, int start, int end)
        {
            var database = connection.GetDatabase();

            var values = await database.SortedSetRangeByRankAsync(sortedKey, start, end);

            var result = new List<T>();

            foreach (var value in values)
            {
                var deserializedValue = JsonSerializer.Deserialize<T>(value);
                result.Add(deserializedValue);
            }

            return result;
        }

        public async Task<long> IncrementValue(string key, long value)
        {
            var database = connection.GetDatabase();

            // it increases the value of our data  
            var data = database.StringIncrementAsync(key, value);

            return await data;

        }
    }
}

