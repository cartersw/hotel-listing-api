using HotelListing.Api.Application.Caching;
using HotelListing.Api.Common.Models.Cache;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Tests.Unit
{
    public class CacheUnitTests
    {
        [Fact]
        public void Set_ThenTryGetValue_ReturnsCached()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cache = new MemoryCacheService(memoryCache);

            cache.Set(
                "country_",
                TestCountries.CountryOneName,
                TimeSpan.FromMinutes(10),
                CacheGroupNames.Country);

            var found = cache.TryGetValue<string>("country_", out var value);

            Assert.True(found);
            Assert.Equal(TestCountries.CountryOneName, value);
        }

        [Fact]
        public void Set_ThenInvalidate_ReturnsNull()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cache = new MemoryCacheService(memoryCache);

            cache.Set(
                "country_",
                TestCountries.CountryOneName,
                TimeSpan.FromMinutes(10),
                CacheGroupNames.Country);

            cache.Invalidate(CacheGroupNames.Country);

            var found = cache.TryGetValue<string>("country_", out var value);

            Assert.False(found);
            Assert.Null(value);
        }
    }
}
