using System.Threading.Tasks;

namespace IoTCenterCore.Localization
{
    public interface ILocalizationService
    {
        Task<string> GetDefaultCultureAsync();

        Task<string[]> GetSupportedCulturesAsync();
    }
}
