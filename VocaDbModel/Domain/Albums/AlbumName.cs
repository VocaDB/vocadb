#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumName : LocalizedStringWithId
	{
		private Album album;

		public AlbumName() { }

		public AlbumName(Album album, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language)
		{
			Album = album;
		}

		public virtual Album Album
		{
			get { return album; }
			set
			{
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual bool Equals(AlbumName another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as AlbumName);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("name '{0}' for {1}", Value, Album);
		}
	}
}
