using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Helpers {

	public static class EnumerableExtender {

		public static Dictionary<T, T2> ToDictionaryWithEmpty<TSource, T, T2>(this IEnumerable<TSource> source, T emptyKey, T2 emptyVal, Func<TSource, T> keySelector, Func<TSource, T2> valueSelector) {

			var vals = new Dictionary<T, T2>();
			vals.Add(emptyKey, emptyVal);
			
			foreach (var item in source)
				vals.Add(keySelector(item), valueSelector(item));

			return vals;

		}

		public static IEnumerable<KeyValuePair<T, T2>> ToKeyValuePairsWithEmpty<TSource, T, T2>(this IEnumerable<TSource> source, T emptyKey, T2 emptyVal, Func<TSource, T> keySelector, Func<TSource, T2> valueSelector) {

			var vals = new List<KeyValuePair<T, T2>>();
			vals.Add(new KeyValuePair<T, T2>(emptyKey, emptyVal));

			foreach (var item in source)
				vals.Add(new KeyValuePair<T, T2>(keySelector(item), valueSelector(item)));

			return vals;

		}

	}

}
