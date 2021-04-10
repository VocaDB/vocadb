#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumName : LocalizedStringWithId
	{
		private Album _album;

		public AlbumName() { }

		public AlbumName(Album album, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language)
		{
			Album = album;
		}

		public virtual Album Album
		{
			get => _album;
			set
			{
				ParamIs.NotNull(() => value);
				_album = value;
			}
		}

#nullable enable
		public virtual bool Equals(AlbumName? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as AlbumName);
		}
#nullable disable

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"name '{Value}' for {Album}";
		}
	}
}
