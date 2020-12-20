#nullable disable

using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search
{
	public class QueryProcessor<T>
	{
		private readonly IDatabaseContext _querySource;

		public QueryProcessor(IDatabaseContext querySource)
		{
			_querySource = querySource;
		}

		public PartialFindResult<T> Query(
			QueryPlan<T> queryPlan, PagingProperties paging,
			Func<IQueryable<T>, IQueryable<T>> orderBy)
		{
			if (!queryPlan.Any())
				return new PartialFindResult<T>(new T[] { }, 0);

			IQueryable<T> entries = null;

			foreach (var filter in queryPlan)
			{
				if (entries == null)
					entries = filter.Query(_querySource);
				else
					entries = filter.Filter(entries, _querySource);
			}

			if (entries == null)
				return new PartialFindResult<T>(new T[] { }, 0);

			/*var queryResult =
				(orderBy != null ? orderBy(entries) : entries)				
				.Distinct();*/
			//var queryResult = entries.Distinct();
			var queryResult = entries;

			var result =
				queryResult
				.Skip(paging.Start)
				.Take(paging.MaxEntries)
				.ToArray();

			return new PartialFindResult<T>(result, queryResult.Count());
		}
	}
}
