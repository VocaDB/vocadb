using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Helpers {

	public static class EnumerableExtender {

		public static IEnumerable<T> Insert<T>(this IEnumerable<T> source, T element) {
			return Enumerable.Repeat(element, 1).Concat(source);
		}

		/// <summary>
		/// Returns the item with the highest value specified by a selector.
		/// </summary>
		/// <typeparam name="TSource">List item type.</typeparam>
		/// <typeparam name="TResult">Selected value type.</typeparam>
		/// <param name="source">Source list. Cannot be null.</param>
		/// <param name="selector">Value selector function. Cannot be null.</param>
		/// <returns>Item with the highest value.</returns>
		public static TSource MaxItem<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> selector) {
		
			var max = source.Max(selector);

			return source.First(t => Equals(selector(t), max));

		} 

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
