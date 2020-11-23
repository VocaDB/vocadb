using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.Mapping.Discussions
{
	public class DiscussionCommentMap : CommentMap<DiscussionComment, DiscussionTopic>
	{
		public DiscussionCommentMap()
		{
			Schema("discussions");
			Table("DiscussionComments");

			Map(m => m.AuthorName).Not.Nullable();

			References(m => m.EntryForComment).Column("[Topic]").Not.Nullable();
		}
	}
}
