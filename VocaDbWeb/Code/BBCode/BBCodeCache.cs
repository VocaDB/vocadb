using System.Web;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Web.Code.BBCode {

	/// <summary>
	/// Utilizes ASP.NET cache to store the results of transformed BBCode.
	/// </summary>
	public class BBCodeCache {

		private readonly BBCodeConverter codeConverter;

		private System.Web.Caching.Cache Cache {
			get {
				return HttpContext.Current.Cache;
			}
		}

		private class CachedValue {

			private readonly string raw;
			private readonly string transformed;

			public CachedValue(string raw, string transformed) {
				this.raw = raw;
				this.transformed = transformed;
			}

			public string Raw {
				get { return raw; }
			}

			public string Transformed {
				get { return transformed; }
			}

		}

		private CachedValue Get(string key) {
			return (CachedValue)Cache[key];
		}

		private void Set(string key, string raw, string transformed) {
			Cache[key] = new CachedValue(raw, transformed);
		}

		public BBCodeCache(BBCodeConverter codeConverter) {
			this.codeConverter = codeConverter;
		}

		/// <summary>
		/// Encodes and caches the unencoded string, using the hash key as the cache key.
		/// The encoded result will be save to the cache for later use.
		/// If possible, it's recommended to use the overload with a custom, more unique cache key.
		/// </summary>
		/// <param name="rawValue">Raw, unencoded string. Can be null or empty, in which case nothing is done.</param>
		/// <returns>Encoded string, from the cache if possible. Can be null or empty.</returns>
		/// <remarks>
		/// Note that using the hash code may cause collisions, but that's not really a problem since the 
		/// contents of the unencoded values are still compared.
		/// </remarks>
		public string GetHtml(string rawValue) {

			if (string.IsNullOrEmpty(rawValue))
				return rawValue;

			return GetHtml(rawValue, (rawValue.Length <= 20 ? rawValue : rawValue.GetHashCode().ToString()));

		}

		public string GetHtml(string rawValue, string key) {

			if (string.IsNullOrEmpty(rawValue))
				return rawValue;

			var cached = Get(key);

			if (cached != null && cached.Raw == rawValue)
				return cached.Transformed;

			var transformed = codeConverter.ConvertToHtml(rawValue);

			Set(key, rawValue, transformed);

			return transformed;

		}

	}

}