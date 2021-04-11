#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Artists
{
	public class ArtistName : LocalizedStringWithId
	{
		private Artist _artist;

		public ArtistName() { }

		public ArtistName(Artist artist, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language)
		{
			Artist = artist;
		}

		public virtual Artist Artist
		{
			get => _artist;
			set
			{
				ParamIs.NotNull(() => value);
				_artist = value;
			}
		}

#nullable enable
		public virtual bool Equals(ArtistName? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as ArtistName);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"name '{Value}' for {Artist}";
		}
#nullable disable
	}
}
