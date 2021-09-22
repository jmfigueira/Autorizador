using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace authorizer_infra.Context
{
    public class PersistContext: IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        private readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(typeof(Func<MemoryCache, object>),
                                    typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
                                    throwOnBindFailure: true) as Func<MemoryCache, object>;

        public PersistContext()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public T Add<T>(Guid key, T t)
        {
            _memoryCache.Set(key, t);

            return _memoryCache.Get<T>(key);
        }

        public T Get<T>(Guid key)
        {
            return _memoryCache.Get<T>(key);
        }

        public bool TryGet<T>(Guid key)
        {
            return _memoryCache.TryGetValue<T>(key, out T value);
        }

        public IEnumerable<T> GetAll<T>()
        {
            List<T> lts = new();

            foreach (var key in ((IDictionary)GetEntriesCollection((MemoryCache)_memoryCache)).Keys)
            {
                if (TryGet<T>((Guid)key))
                    lts.Add(Get<T>((Guid)key));
            }

            return lts;
        }

        public void Update<T>(Guid key, T t)
        {
            _memoryCache.Remove(key);

            _memoryCache.Set(key, t);
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }
    }
}