using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.Helpers {

	public static class EntryWithStatusQueryableExtender {

		public static IQueryable<T> WhereStatusIs<T>(this IQueryable<T> query, EntryStatus? status) where T : class, IEntryWithStatus {

			if (!status.HasValue)
				return query;

			return query.Where(a => a.Status == status);

		}

	}

}
