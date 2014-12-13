using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class QueryableForReleaseEventExtender {

		public static IQueryable<ReleaseEvent> WhereHasName(this IQueryable<ReleaseEvent> query, SearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(t => t.Name == textQuery.Query);
				case NameMatchMode.StartsWith:
					return query.Where(t => t.Name.StartsWith(textQuery.Query));
				default:
					return query.Where(t => t.Name.Contains(textQuery.Query));
			}

		}

		public static IQueryable<ReleaseEvent> OrderBy(this IQueryable<ReleaseEvent> query, EventSortRule sortRule) {

			switch (sortRule) {
				case EventSortRule.Date:
					return query.OrderByDescending(r => r.Date);
				case EventSortRule.Name:
					return query.OrderBy(r => r.Name);
				case EventSortRule.SeriesName:
					return query
						.OrderBy(r => r.Series.Name)
						.ThenBy(r => r.SeriesNumber);
			}

			return query;

		}

		public static IQueryable<ReleaseEvent> WhereHasSeries(this IQueryable<ReleaseEvent> query, int seriesId) {
			
			if (seriesId == 0)
				return query;

			return query.Where(e => e.Series.Id == seriesId);

		} 

	}

	public enum EventSortRule {
		
		None,

		Name,

		Date,

		SeriesName

	}

}
