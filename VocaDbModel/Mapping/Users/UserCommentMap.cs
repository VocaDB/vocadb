using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class UserCommentMap : CommentMap<UserComment, User> {

		public UserCommentMap() {

			References(m => m.EntryForComment).Column("[User]").Not.Nullable();

		}

	}
}
