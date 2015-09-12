using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// Comment created on user's profile.
	/// </summary>
	public class UserComment : GenericComment<User> {

		public UserComment() { }

		public UserComment(User user, string message, AgentLoginData loginData)
			: base(user, message, loginData) {}

		public override void OnDelete() {
			EntryForComment.Comments.Remove(this);
		}

	}

}
