#nullable disable

using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class EntryWithStatusQueryableExtensions
	{
		public static IQueryable<T> WhereStatusIs<T>(this IQueryable<T> query, EntryStatus? status) where T : class, IEntryWithStatus
		{
			if (!status.HasValue)
				return query;

			return query.Where(a => a.Status == status);
		}
	}
}
