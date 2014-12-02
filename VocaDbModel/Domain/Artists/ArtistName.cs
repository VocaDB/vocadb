using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistName : LocalizedStringWithId {

		private Artist artist;

		public ArtistName() {}

		public ArtistName(Artist artist, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language) {

			Artist = artist;

		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual bool Equals(ArtistName another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistName);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return string.Format("name '{0}' for {1}", Value, Artist);
		}

	}

}
