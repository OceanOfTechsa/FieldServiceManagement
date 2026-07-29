using CacheManager.Core;
using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.Services;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class CacheModule
    {
        public static IServiceCollection AddCacheModule(this IServiceCollection services, IWebHostEnvironment env)
        {
            var connectionString = AppSettings.GetRedisConnectionString();
            var split = connectionString.Split(',');

            services.AddCacheManagerConfiguration(builder => builder
                .WithRedisConfiguration("redis", config =>
                {
                    config.WithDatabase(Convert.ToInt16(split[5]))
                          .WithEndpoint(split[0], Convert.ToInt16(split[1]))
                          .WithPassword(split[2]);

                    if (!env.IsDevelopment()) config.WithSsl();
                })
                .WithJsonSerializer()
                .WithMaxRetries(100)
                .WithRetryTimeout(50)
                .WithRedisBackplane("redis")
                .WithRedisCacheHandle("redis", true)
                .Build());

            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddCacheManager();

            // Build a temporary provider just to get the instance and initialize the static helper
            var serviceProvider = services.BuildServiceProvider();
            var cache = serviceProvider.GetRequiredService<ICacheManager<object>>();
            CacheBusiness.Initialize(cache);

            return services;
        }
    }
}
