using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMemoryService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        
        // we can use that for accessing to a range of values (for example  for mostviewed ads)
        Task<IEnumerable<T>> RangeSortedSet<T>(string sortedKey, int start, int end);

        Task<long> IncrementValue(string key,long value);
    }
}
