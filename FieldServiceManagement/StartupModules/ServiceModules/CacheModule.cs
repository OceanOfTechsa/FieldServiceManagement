using CacheManager.Core;
using FieldServiceManagement.Business.Configuration;

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

            return services;
        }
    }
}
