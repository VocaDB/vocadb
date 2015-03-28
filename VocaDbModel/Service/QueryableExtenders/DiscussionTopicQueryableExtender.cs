using System.Linq;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class DiscussionTopicQueryableExtender {

		public static IQueryable<DiscussionTopic> OrderBy(this IQueryable<DiscussionTopic> query, DiscussionTopicSortRule sort) {

			switch (sort) {
				case DiscussionTopicSortRule.Name:
					return query.OrderBy(d => d.Name);
				case DiscussionTopicSortRule.DateCreated:
					return query.OrderByDescending(d => d.Created);
			}

			return query;

		} 

	}

	public enum DiscussionTopicSortRule {
		
		None,

		Name,

		DateCreated

	}

}
