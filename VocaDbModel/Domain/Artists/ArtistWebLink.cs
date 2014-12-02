namespace VocaDb.Model.Domain.Artists {

	public class ArtistWebLink : WebLink {

		private Artist artist;

		public ArtistWebLink() {}

		public ArtistWebLink(Artist artist, string description, string url, WebLinkCategory category) 
			: base(description, url, category) {

			Artist = artist;

		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual bool Equals(ArtistWebLink another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistWebLink);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return base.ToString() + " for " + Artist;
		}

	}

}
