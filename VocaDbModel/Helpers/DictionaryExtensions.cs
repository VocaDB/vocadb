#nullable disable

using System.Collections.Generic;

namespace VocaDb.Model.Helpers
{
	public static class DictionaryExtensions
	{
		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
		{
			return dictionary.TryGetValue(key, out var val) ? val : default;
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
		{
			return dictionary.TryGetValue(key, out var val) ? val : default;
		}
	}
}
