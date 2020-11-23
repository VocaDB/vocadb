using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class EntryReportQueryableExtensions
	{
		public static IQueryable<EntryReport> OrderBy(this IQueryable<EntryReport> query, EntryReportSortRule sortRule)
		{
			switch (sortRule)
			{
				case EntryReportSortRule.CloseDate:
					return query.OrderByDescending(e => e.ClosedAt ?? e.Created);
				case EntryReportSortRule.Created:
					return query.OrderByDescending(e => e.Created);
			}

			return query;
		}
	}

	public enum EntryReportSortRule
	{
		None,
		Created,
		CloseDate
	}
}
