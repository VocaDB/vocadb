using System.Collections.Generic;

namespace VocaDb.Model.Helpers {

	public static class DictionaryExtensions {

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
			return dictionary.TryGetValue(key, out var val) ? val : default;
		}

	}

}
