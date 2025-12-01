using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterCore.DynamicCache
{
    public interface ICacheContextProvider
    {
        Task PopulateContextEntriesAsync(IEnumerable<string> contexts, List<CacheContextEntry> entries);
    }
}
