using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class GenericQueryableExtender {

		public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, SortDirection direction) {

			return direction == SortDirection.Ascending ?
				query.OrderBy(keySelector) :
				query.OrderByDescending(keySelector);

		}

		public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, SortDirection direction) {

			return direction == SortDirection.Ascending ?
				query.ThenBy(keySelector) :
				query.ThenByDescending(keySelector);

		}

		/// <summary>
		/// Filters query for paging.
		/// </summary>
		/// <typeparam name="T">Queried root entity.</typeparam>
		/// <param name="query">Query. Cannot be null.</param>
		/// <param name="paging">Paging properties. Can be null.</param>
		/// <param name="allowEmpty">Allow empty result if MaxResults is less than 1</param>
		/// <returns>Query which is filtered for paging.</returns>
		public static IQueryable<T> Paged<T>(this IQueryable<T> query, PagingProperties paging, bool allowEmpty = false) {
			
			if (paging == null)
				return query;

			if (allowEmpty && paging.MaxEntries <= 0)
				return new T[0].AsQueryable();

			var start = Math.Max(paging.Start, 0);
			var maxEntries = Math.Max(paging.MaxEntries, 1); // Take for database must be >= 1

			return query.Skip(start).Take(maxEntries);

		}

	}

	public enum SortDirection {
		Ascending,
		Descending
	}


}
