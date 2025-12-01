using System.Collections.Generic;
using System.Threading.Tasks;
using IoTCenterCore.Environment.Extensions.Features;

namespace IoTCenterCore.Environment.Shell
{
    public static class ShellFeaturesManagerExtensions
    {
        public static Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(this IShellFeaturesManager shellFeaturesManager,
            IEnumerable<IFeatureInfo> features)
        {
            return shellFeaturesManager.EnableFeaturesAsync(features, false);
        }

        public static async Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(this IShellFeaturesManager shellFeaturesManager,
            IEnumerable<IFeatureInfo> features, bool force)
        {
            var (featuresToDisable, featuresToEnable) = await shellFeaturesManager.UpdateFeaturesAsync(new IFeatureInfo[0], features, force);
            return featuresToEnable;
        }

        public static Task<IEnumerable<IFeatureInfo>> DisableFeaturesAsync(this IShellFeaturesManager shellFeaturesManager,
            IEnumerable<IFeatureInfo> features)
        {
            return shellFeaturesManager.DisableFeaturesAsync(features, false);
        }

        public static async Task<IEnumerable<IFeatureInfo>> DisableFeaturesAsync(this IShellFeaturesManager shellFeaturesManager,
            IEnumerable<IFeatureInfo> features, bool force)
        {
            var (featuresToDisable, featuresToEnable) = await shellFeaturesManager.UpdateFeaturesAsync(features, new IFeatureInfo[0], force);
            return featuresToDisable;
        }
    }
}
