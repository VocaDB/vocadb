using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Users {

	public class UserWebLink : WebLink {

		private User user;

		public UserWebLink() { }

		public UserWebLink(User user, WebLinkContract contract)
			: base(contract) {

			User = user;

		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual bool Equals(UserWebLink another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as UserWebLink);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return string.Format("{0} for {1}", base.ToString(), User);
		}

	}

}
