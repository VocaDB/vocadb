using System;
using System.Runtime.Caching;

namespace VocaDb.Model.Domain.Caching {

	public static class ObjectCacheExtender {

		public static T GetOrInsert<T>(this ObjectCache cache, string key, CacheItemPolicy cacheItemPolicy, Func<T> func) {
			
			// Note: not thread safe
			if (cache.Contains(key))
				return (T)cache.Get(key);

			var item = func();
			cache.Add(key, item, cacheItemPolicy);

			return item;

		}

	}

	public static class CachePolicy {

		public static CacheItemPolicy AbsoluteExpiration(TimeSpan fromNow) {
			return new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now + fromNow };
		}

		public static CacheItemPolicy AbsoluteExpiration(int hours) {
			return AbsoluteExpiration(TimeSpan.FromHours(hours));
		}

		public static CacheItemPolicy Never() {
			return new CacheItemPolicy();
		}

	}

}
