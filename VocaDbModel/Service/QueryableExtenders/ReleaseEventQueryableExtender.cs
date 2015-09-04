using System;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class ReleaseEventQueryableExtender {

		public static IQueryable<ReleaseEvent> WhereHasName(this IQueryable<ReleaseEvent> query, SearchTextQuery textQuery) {

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

		public static IQueryable<ReleaseEvent> WhereDateIsBetween(this IQueryable<ReleaseEvent> query, DateTime? begin, DateTime? end) {
			
			if (begin.HasValue && end.HasValue)
				return query.Where(e => e.Date != null && e.Date >= begin && e.Date < end);

			if (begin.HasValue)
				return query.Where(e => e.Date != null && e.Date >= begin);

			if (end.HasValue)
				return query.Where(e => e.Date != null && e.Date < end);

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
