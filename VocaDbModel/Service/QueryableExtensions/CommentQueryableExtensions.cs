using System.Linq;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public enum CommentSortRule
	{
		CreateDateDescending,

		CreateDate,
	}

	public static class CommentQueryableExtensions
	{
		public static IQueryable<Comment> OrderBy(this IQueryable<Comment> queryable, CommentSortRule sortRule) => sortRule switch
		{
			CommentSortRule.CreateDate => queryable.OrderBy(activityEntry => activityEntry.Created),
			CommentSortRule.CreateDateDescending => queryable.OrderByDescending(activityEntry => activityEntry.Created),
			_ => queryable,
		};
	}
}
