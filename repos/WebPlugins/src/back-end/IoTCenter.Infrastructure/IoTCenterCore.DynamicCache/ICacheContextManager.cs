using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterCore.DynamicCache
{
    public interface ICacheContextManager
    {
        Task<IEnumerable<CacheContextEntry>> GetDiscriminatorsAsync(IEnumerable<string> contexts);
    }
}
