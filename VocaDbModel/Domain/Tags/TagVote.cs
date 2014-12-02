using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags {

	public abstract class TagVote {

		private User user;

		protected TagVote() { }

		protected TagVote(User user) {
			User = user;
		}

		public virtual long Id { get; set; }

		public abstract TagUsage UsageBase { get; }

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual bool Equals(TagVote another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as TagVote);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return string.Format("Vote for {0} by {1}", UsageBase, User);
		}

	}
}
