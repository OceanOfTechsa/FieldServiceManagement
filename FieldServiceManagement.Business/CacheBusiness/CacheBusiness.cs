// Business/Services/CacheBusiness.cs
using CacheManager.Core;
using FieldServiceManagement.Business.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace FieldServiceManagement.Business.Services
{
    public static class CacheBusiness
    {
        private static ICacheManager<object>? _cache;

        /// <summary>
        /// Call this once during startup to initialize the cache instance
        /// </summary>
        public static void Initialize(ICacheManager<object> cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private static ICacheManager<object> Cache
        {
            get
            {
                if (_cache == null)
                    throw new InvalidOperationException("CacheBusiness is not initialized. Call Initialize() first.");
                return _cache;
            }
        }

        /// <summary>
        /// Flushes the entire cache
        /// </summary>
        public static void FlushAll()
        {
            Cache.Clear();
        }

        /// <summary>
        /// Clears a specific region
        /// </summary>
        public static void FlushByRegion(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region is required", nameof(region));

            Cache.ClearRegion(region);
        }

        /// <summary>
        /// Removes a single key (optionally inside a region)
        /// </summary>
        public static void FlushByKey(string key, string? region = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key is required", nameof(key));

            if (string.IsNullOrWhiteSpace(region))
                Cache.Remove(key);
            else
                Cache.Remove(key, region);
        }

        /// <summary>
        /// Forces revalidation of a single key (next request will rebuild it)
        /// </summary>
        public static void Revalidate(string key, string? region = null)
        {
            FlushByKey(key, region);
        }

        /// <summary>
        /// Forces revalidation of an entire region
        /// </summary>
        public static void RevalidateRegion(string region)
        {
            FlushByRegion(region);
        }
    }
}