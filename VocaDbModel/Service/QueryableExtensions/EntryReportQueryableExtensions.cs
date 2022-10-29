using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtensions;

public static class EntryReportQueryableExtensions
{
	public static IQueryable<EntryReport> OrderBy(this IQueryable<EntryReport> query, EntryReportSortRule sortRule) => sortRule switch
	{
		EntryReportSortRule.CloseDate => query.OrderByDescending(e => e.ClosedAtUtc ?? e.CreatedUtc),
		EntryReportSortRule.Created => query.OrderByDescending(e => e.CreatedUtc),
		_ => query,
	};
}

public enum EntryReportSortRule
{
	None,
	Created,
	CloseDate
}
