#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Service.Search
{
	public class QueryPlan<TEntry> : IEnumerable<ISearchFilter<TEntry>>
	{
		private readonly List<ISearchFilter<TEntry>> _filters;

		public QueryPlan(IEnumerable<ISearchFilter<TEntry>> filters)
		{
			this._filters = filters.OrderBy(f => f.Cost).ToList();
		}

		public IEnumerator<ISearchFilter<TEntry>> GetEnumerator()
		{
			return _filters.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
