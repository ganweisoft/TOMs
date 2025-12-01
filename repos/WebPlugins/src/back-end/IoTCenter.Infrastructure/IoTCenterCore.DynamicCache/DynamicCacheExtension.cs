using IoTCenterCore.DynamicCache;
using IoTCenterCore.DynamicCache.Abstractions;
using IoTCenterCore.DynamicCache.CacheContextProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamicCacheExtension
    {
        public static IServiceCollection AddDefaultDynamicCache(this IServiceCollection services)
        {
            services.AddScoped<ICacheContextManager, CacheContextManager>();

            services.AddScoped<ICacheContextProvider, DefaultCacheContextProvider>();

            services.AddDistributedMemoryCache();

            services.AddScoped<DefaultDynamicCacheServiceImpl>();
            
            services.AddScoped<IDynamicCacheService>(sp => sp.GetRequiredService<DefaultDynamicCacheServiceImpl>());

            services.AddSingleton<IDynamicCache, DefaultDynamicCache>();

            services.AddSingleton(typeof(DynamicCacheHelperService<>));

            return services;
        }
    }
}
