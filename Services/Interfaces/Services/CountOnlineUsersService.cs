using Entities.Common.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class CountOnlineUsersService : ICountOnlineUsersService
    {
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _cacheSettings;
        public CountOnlineUsersService(IDistributedCache cache, IOptions<CacheSettings> cacheSettings)
        {
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task MarkUserAsOnline(string userId)
        {
            var key = "OnlineUsers";

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            };

            var existingOnlineUsers = await _cache.GetStringAsync(key);
            HashSet<string> onlineUsers;

            if (string.IsNullOrEmpty(existingOnlineUsers))
            {
                onlineUsers = new HashSet<string>();
            }
            else
            {
                onlineUsers = JsonConvert.DeserializeObject<HashSet<string>>(existingOnlineUsers);
            }

            onlineUsers.Add(userId);

            var serializedData = JsonConvert.SerializeObject(onlineUsers);

            // Add the user to the "online_users" Redis Set asynchronously
            await _cache.SetStringAsync(key, serializedData, options);
        }

        public async Task MarkUserAsOffline(string userId)
        {
            var key = "OnlineUsers";

            // Retrieve the current set of online users or create a new HashSet
            var existingOnlineUsers = await _cache.GetStringAsync(key);
            HashSet<string> onlineUsers;

            if (string.IsNullOrEmpty(existingOnlineUsers))
            {
                onlineUsers = new HashSet<string>();
            }
            else
            {
                onlineUsers = JsonConvert.DeserializeObject<HashSet<string>>(existingOnlineUsers);
            }

            // Remove the user ID from the set
            onlineUsers?.Remove(userId);

            // Store the updated set back in the cache
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // Adjust the expiration as needed
            };

            // Serialize the HashSet to JSON before storing it
            var serializedOnlineUsers = JsonConvert.SerializeObject(onlineUsers);
            await _cache.SetStringAsync(key, serializedOnlineUsers, options);
        }


        public async Task<int> CountOnlineUsers()
        {
            var key = "OnlineUsers";

            // Retrieve the current set of online users or create a new HashSet
            var existingOnlineUsers = await _cache.GetStringAsync(key);
            HashSet<string> onlineUsers;

            if (string.IsNullOrEmpty(existingOnlineUsers))
            {
                onlineUsers = new HashSet<string>();
            }
            else
            {
                onlineUsers = JsonConvert.DeserializeObject<HashSet<string>>(existingOnlineUsers);
            }
            int count = onlineUsers.Count != 0 ? onlineUsers.Count() : 0;

            return count;
        }
    }
}
