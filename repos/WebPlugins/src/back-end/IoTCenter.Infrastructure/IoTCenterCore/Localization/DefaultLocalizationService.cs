using System.Globalization;
using System.Threading.Tasks;

namespace IoTCenterCore.Localization
{
    public class DefaultLocalizationService : ILocalizationService
    {
        private static readonly Task<string> DefaultCulture = Task.FromResult(CultureInfo.InstalledUICulture.Name);
        private static readonly Task<string[]> SupportedCultures = Task.FromResult(new[] { CultureInfo.InstalledUICulture.Name });

        public Task<string> GetDefaultCultureAsync() => DefaultCulture;

        public Task<string[]> GetSupportedCulturesAsync() => SupportedCultures;
    }
}
