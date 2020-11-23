using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class CommentQueryableExtensions
	{
		public static IQueryable<GenericComment<T>> WhereNotDeleted<T>(this IQueryable<GenericComment<T>> query) where T : class, IEntryWithNames => query.Where(c => !c.Deleted);
	}
}
