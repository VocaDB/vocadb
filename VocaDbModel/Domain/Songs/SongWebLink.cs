namespace VocaDb.Model.Domain.Songs {

	public class SongWebLink : WebLink {

		private Song song;

		public SongWebLink() { }

		public SongWebLink(Song song, string description, string url, WebLinkCategory category)
			: base(description, url, category) {

			Song = song;

		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual bool Equals(SongWebLink another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as SongWebLink);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return base.ToString() + " for " + Song;
		}

	}

}
