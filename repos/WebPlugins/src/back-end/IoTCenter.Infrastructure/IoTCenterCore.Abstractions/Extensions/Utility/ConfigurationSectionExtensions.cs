using Microsoft.Extensions.Configuration;

namespace IoTCenterCore.Environment.Extensions.Utility
{
    public static class ConfigurationSectionExtensions
    {
        public static IConfigurationSection GetSectionCompat(this IConfiguration configuration, string key)
        {
            var section = configuration.GetSection(key);

            return section.Exists()
                ? section
                : key.Contains('_')
                    ? configuration.GetSection(key.Replace('_', '.'))
                    : section;
        }
    }
}
