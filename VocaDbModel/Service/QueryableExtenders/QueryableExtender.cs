using System;
using System.Linq;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class QueryableExtender {

		/// <summary>
		/// Filters query for paging.
		/// </summary>
		/// <typeparam name="T">Queried root entity.</typeparam>
		/// <param name="query">Query. Cannot be null.</param>
		/// <param name="paging">Paging properties. Can be null.</param>
		/// <returns>Query which is filtered for paging.</returns>
		public static IQueryable<T> Paged<T>(this IQueryable<T> query, PagingProperties paging) {
			
			if (paging == null)
				return query;

			var start = Math.Max(paging.Start, 0);
			var maxEntries = Math.Max(paging.MaxEntries, 1);

			return query.Skip(start).Take(maxEntries);

		}

	}

}
