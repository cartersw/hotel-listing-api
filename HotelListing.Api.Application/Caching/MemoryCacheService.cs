using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HotelListing.Api.Application.Caching
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _cache;

        private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokenSources = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key,
            T value,
            TimeSpan expiration,
            string group)
        {

            var tokenSource = _tokenSources.GetOrAdd(
                group,
                _ => new CancellationTokenSource());

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration)
                .AddExpirationToken(
                    new CancellationChangeToken(
                            tokenSource.Token));

            _cache.Set(key, value, options);
        }

        public bool TryGetValue<T>(string key, out T? value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Invalidate(string group)
        {
            if(_tokenSources.TryRemove(group, out var tokenSource))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }
    }
}
