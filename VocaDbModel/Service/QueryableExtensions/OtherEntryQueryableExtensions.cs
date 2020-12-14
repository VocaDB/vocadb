#nullable disable

using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class OtherEntryQueryableExtensions
	{
		public static IQueryable<T> WhereIsDeleted<T>(this IQueryable<T> query, bool deleted) where T : IDeletableEntry
			=> query.Where(m => m.Deleted == deleted);

		public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query) where T : IDeletableEntry
			=> query.Where(m => !m.Deleted);
	}
}
