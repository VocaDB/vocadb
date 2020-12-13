using System.Linq;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public enum ActivityEntrySortRule
	{
		CreateDateDescending,

		CreateDate,
	}

	public static class ActivityEntryQueryableExtensions
	{
		public static IQueryable<ActivityEntry> OrderBy(this IQueryable<ActivityEntry> queryable, ActivityEntrySortRule sortRule) => sortRule switch
		{
			ActivityEntrySortRule.CreateDate => queryable.OrderBy(activityEntry => activityEntry.CreateDate),
			ActivityEntrySortRule.CreateDateDescending => queryable.OrderByDescending(activityEntry => activityEntry.CreateDate),
			_ => queryable,
		};
	}
}
