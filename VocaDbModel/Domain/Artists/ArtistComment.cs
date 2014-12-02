using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistComment : Comment {

		private Artist artist;

		public ArtistComment() { }

		public ArtistComment(Artist artist, string message, User user)
			: base(message, new AgentLoginData(user)) {

			Artist = artist;

		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public override IEntryWithNames Entry {
			get { return Artist; }
		}

		public virtual bool Equals(ArtistComment another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistComment);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return string.Format("comment [{0}] for " + Artist, Id);
		}

	}

}
