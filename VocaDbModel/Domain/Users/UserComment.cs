using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// Comment created on user's profile.
	/// </summary>
	public class UserComment : Comment {

		private User user;

		public UserComment() { }

		public UserComment(User user, string message, AgentLoginData loginData)
			: base(message, loginData) {

			User = user;

		}

		public override IEntryWithNames Entry {
			get { return User; }
		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual bool Equals(UserComment another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as UserComment);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override void OnDelete() {
			User.Comments.Remove(this);
		}

		public override string ToString() {
			return string.Format("comment [{0}] for {1}", Id, User);
		}

	}

}
