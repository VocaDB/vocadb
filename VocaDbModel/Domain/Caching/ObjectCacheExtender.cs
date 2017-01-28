using System;
using System.Runtime.Caching;

namespace VocaDb.Model.Domain.Caching {

	public static class ObjectCacheExtender {

		/// <summary>
		/// Get cache item, or insert if it doesn't exist.
		/// </summary>
		/// <typeparam name="T">Return type.</typeparam>
		/// <param name="cache">Cache. Cannot be null.</param>
		/// <param name="key">Cache key. Cannot be null or empty.</param>
		/// <param name="cacheItemPolicy">Cache item policy. Cannot be null.</param>
		/// <param name="func">Factory function for getting the data to cache if it doesn't exist.</param>
		/// <param name="allowCaching">
		/// Function for testing whether the new data should be cached. 
		/// Can be null.
		/// If this is null or it returns true, the new data is cached. 
		/// If the function is not null and it returns false, the new data is NOT cached.
		/// This can be used for conditional caching for example when the number of items is small.
		/// </param>
		/// <returns>Data from the cache or factory function.</returns>
		/// <remarks>
		/// This method is not thread safe in the sense that the data factory method may be called multiple times from different threads.
		/// </remarks>
		public static T GetOrInsert<T>(this ObjectCache cache, string key, CacheItemPolicy cacheItemPolicy, Func<T> func, Func<T, bool> allowCaching = null) {
			
			// Note: not thread safe
			if (cache.Contains(key))
				return (T)cache.Get(key);

			var item = func();

			var addCache = allowCaching == null || allowCaching(item);

			if (addCache) {
				cache.Add(key, item, cacheItemPolicy);
			}

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

		/// <summary>
		/// Cache never expires.
		/// </summary>
		public static CacheItemPolicy Never() {
			return new CacheItemPolicy();
		}

	}

}
