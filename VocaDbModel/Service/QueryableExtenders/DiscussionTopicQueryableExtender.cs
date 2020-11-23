using System.Linq;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Service.QueryableExtenders
{
	public static class DiscussionTopicQueryableExtender
	{
		public static IQueryable<DiscussionTopic> OrderBy(this IQueryable<DiscussionTopic> query, DiscussionTopicSortRule sort)
		{
			switch (sort)
			{
				case DiscussionTopicSortRule.Name:
					return query.OrderBy(d => d.Name);
				case DiscussionTopicSortRule.DateCreated:
					return query.OrderByDescending(d => d.Created);
				case DiscussionTopicSortRule.LastCommentDate:
					return query.OrderByDescending(d => d.Comments.Max(c => c.Created));
			}

			return query;
		}

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
