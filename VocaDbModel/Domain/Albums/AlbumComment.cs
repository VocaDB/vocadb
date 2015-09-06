using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumComment : Comment {

		private Album album;

		public AlbumComment() {}

		public AlbumComment(Album album, string message, AgentLoginData loginData)
			: base(message, loginData) {

			Album = album;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value; 
			}
		}

		public override IEntryWithNames Entry {
			get { return Album; }
		}

		public virtual bool Equals(AlbumComment another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as AlbumComment);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override void OnDelete() {
			Album.Comments.Remove(this);
		}

		public override string ToString() {
			return string.Format("comment [{0}] for " + Album, Id);
		}

	}
}
