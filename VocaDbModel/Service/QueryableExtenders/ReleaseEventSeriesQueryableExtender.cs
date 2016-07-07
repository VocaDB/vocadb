using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class ReleaseEventSeriesQueryableExtender {

		public static IQueryable<ReleaseEventSeries> WhereHasName(this IQueryable<ReleaseEventSeries> query, SearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(t => t.Name == textQuery.Query);
				case NameMatchMode.StartsWith:
					return query.Where(t => t.Name.StartsWith(textQuery.Query));
				case NameMatchMode.Words:
					return textQuery.Words
						.Take(FindHelpers.MaxSearchWords)
						.Aggregate(query, (q, word) => q.Where(list => list.Name.Contains(word)));
				default:
					return query.Where(t => t.Name.Contains(textQuery.Query));
			}

		}
	}
}
