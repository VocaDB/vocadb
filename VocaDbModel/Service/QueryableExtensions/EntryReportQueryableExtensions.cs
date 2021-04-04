using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class EntryReportQueryableExtensions
	{
		public static IQueryable<EntryReport> OrderBy(this IQueryable<EntryReport> query, EntryReportSortRule sortRule) => sortRule switch
		{
			EntryReportSortRule.CloseDate => query.OrderByDescending(e => e.ClosedAt ?? e.Created),
			EntryReportSortRule.Created => query.OrderByDescending(e => e.Created),
			_ => query,
		};
	}

	public enum EntryReportSortRule
	{
		None,
		Created,
		CloseDate
	}
}
