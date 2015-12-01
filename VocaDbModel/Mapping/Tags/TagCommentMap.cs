using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagCommentMap : CommentMap<TagComment, Tag> {

		public TagCommentMap() {
			References(m => m.EntryForComment).Column("[Tag]").Not.Nullable();
		}

	}

}
