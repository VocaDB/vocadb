using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class OtherEntryQueryableExtender {

		public static IQueryable<T> WhereIsDeleted<T>(this IQueryable<T> query, bool deleted) where T : IDeletableEntry 
			=> query.Where(m => m.Deleted == deleted);

		public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query) where T : IDeletableEntry 
			=> query.Where(m => !m.Deleted);

	}
}
