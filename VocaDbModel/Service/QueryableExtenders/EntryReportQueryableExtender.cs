using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class EntryReportQueryableExtender {

		public static IQueryable<EntryReport> OrderBy(this IQueryable<EntryReport> query, EntryReportSortRule sortRule) {

			switch (sortRule) {
				case EntryReportSortRule.CloseDate:
					return query.OrderByDescending(e => e.CloseDate ?? e.Created);
				case EntryReportSortRule.Created:
					return query.OrderByDescending(e => e.Created);
			}

			return query;

		}

	}

	public enum EntryReportSortRule {
		None,
		Created,
		CloseDate
	}

}
