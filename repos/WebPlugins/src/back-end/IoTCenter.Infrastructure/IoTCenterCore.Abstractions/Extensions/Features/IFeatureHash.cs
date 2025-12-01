using System.Threading.Tasks;

namespace IoTCenterCore.Environment.Extensions.Features
{
    public interface IFeatureHash
    {
        Task<int> GetFeatureHashAsync();

        Task<int> GetFeatureHashAsync(string featureId);
    }
}
