using System.Linq;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class DiscussionTopicQueryableExtensions
	{
		public static IQueryable<DiscussionTopic> OrderBy(this IQueryable<DiscussionTopic> query, DiscussionTopicSortRule sort) => sort switch
		{
			DiscussionTopicSortRule.Name => query.OrderBy(d => d.Name),
			DiscussionTopicSortRule.DateCreated => query.OrderByDescending(d => d.Created),
			DiscussionTopicSortRule.LastCommentDate => query.OrderByDescending(d => d.Comments.Max(c => c.Created)),
			_ => query,
		};

		public static IQueryable<DiscussionTopic> WhereIsInFolder(this IQueryable<DiscussionTopic> query, int? folderId)
		{
			if (folderId == null)
				return query;

			return query.Where(t => t.Folder.Id == folderId);
		}
	}

	public enum DiscussionTopicSortRule
	{
		None,

		Name,

		DateCreated,

		LastCommentDate
	}
}
