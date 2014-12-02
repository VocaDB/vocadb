namespace VocaDb.Model.Domain.Albums {

	public class AlbumWebLink : WebLink {

		private Album album;

		public AlbumWebLink() { }

		public AlbumWebLink(Album album, string description, string url, WebLinkCategory category)
			: base(description, url, category) {

			Album = album;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual bool Equals(AlbumWebLink another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as AlbumWebLink);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return base.ToString() + " for " + Album;
		}

	}

}
