using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace IoTCenterCore.DynamicCache.Abstractions
{
    public interface IDynamicCache
    {
        Task<byte[]> GetAsync(string key);
        Task RemoveAsync(string key);
        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options);
    }
}