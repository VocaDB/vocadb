using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Songs {

	public class SongComment : Comment {

		private Song song;

		public SongComment() { }

		public SongComment(Song song, string message, AgentLoginData loginData)
			: base(message, loginData) {

			Song = song;

		}

		public override IEntryWithNames Entry {
			get { return Song; }
		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual bool Equals(SongComment another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as SongComment);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return string.Format("comment [{0}] for " + Song, Id);
		}

	}

}
