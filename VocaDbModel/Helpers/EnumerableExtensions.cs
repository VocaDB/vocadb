#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Helpers
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Distinct<T, T2>(this IEnumerable<T> source, Func<T, T2> func, IEqualityComparer<T2> propertyEquality)
		{
			var comparer = new DistinctPropertyEqualityComparer<T, T2>(func, propertyEquality);
			return source.Distinct(comparer);
		}

		public static IEnumerable<T> Distinct<T, T2>(this IEnumerable<T> source, Func<T, T2> func)
		{
			var comparer = new DistinctPropertyEqualityComparer<T, T2>(func);
			return source.Distinct(comparer);
		}

		public static void ForEach<T, T2>(this IEnumerable<T> source, Func<T, T2> func)
		{
			// Note: if you need the return value, just use Select
			foreach (var item in source)
				func(item);
		}

		public static IEnumerable<T> Insert<T>(this IEnumerable<T> source, T element)
		{
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
		public static TSource MaxItem<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> selector)
		{
			var max = source.Max(selector);

			return source.First(t => Equals(selector(t), max));
		}

		/// <summary>
		/// See https://stackoverflow.com/a/2165611
		/// </summary>
		public static T? MinOrNull<T>(this IEnumerable<T> source) where T : struct => source.Cast<T?>().Min();

		public static T[] OrderByIds<T>(this IEnumerable<T> entries, int[] idList) where T : IEntryWithIntId
		{
			return CollectionHelper.SortByIds(entries, idList);
		}

		public static Dictionary<T, T2> ToDictionaryWithEmpty<TSource, T, T2>(this IEnumerable<TSource> source, T emptyKey, T2 emptyVal, Func<TSource, T> keySelector, Func<TSource, T2> valueSelector)
		{
			var vals = new Dictionary<T, T2>();
			vals.Add(emptyKey, emptyVal);

			foreach (var item in source)
				vals.Add(keySelector(item), valueSelector(item));

			return vals;
		}

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> enumerable) => enumerable.Where(i => i != null);

		public static IEnumerable<string> WhereIsNotNullOrEmpty(this IEnumerable<string> enumerable)
		{
			return enumerable.Where(s => !string.IsNullOrEmpty(s));
		}
	}

	public class DistinctPropertyEqualityComparer<T, T2> : IEqualityComparer<T>
	{
		private readonly IEqualityComparer<T2> _propertyEquality;
		private readonly Func<T, T2> _func;

		public DistinctPropertyEqualityComparer(Func<T, T2> func, IEqualityComparer<T2> propertyEquality)
		{
			_func = func;
			_propertyEquality = propertyEquality;
		}

		public DistinctPropertyEqualityComparer(Func<T, T2> func)
			: this(func, EqualityComparer<T2>.Default) { }

		public bool Equals(T x, T y)
		{
			if (ReferenceEquals(x, y))
				return true;

			return _propertyEquality.Equals(_func(x), _func(y));
		}

		public int GetHashCode(T obj)
		{
			return _propertyEquality.GetHashCode(_func(obj));
		}
	}
}
