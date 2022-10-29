using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Service.QueryableExtensions;

public enum CommentSortRule
{
	CreateDateDescending,

	CreateDate,
}

public static class CommentQueryableExtensions
{
	public static IQueryable<Comment> OrderBy(this IQueryable<Comment> queryable, CommentSortRule sortRule) => sortRule switch
	{
		CommentSortRule.CreateDate => queryable.OrderBy(activityEntry => activityEntry.CreatedUtc),
		CommentSortRule.CreateDateDescending => queryable.OrderByDescending(activityEntry => activityEntry.CreatedUtc),
		_ => queryable,
	};
}
