#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs
{
	public class SongName : LocalizedStringWithId
	{
		private Song song;

		public SongName() { }

		public SongName(Song song, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language)
		{
			Song = song;
		}

		public virtual Song Song
		{
			get { return song; }
			set
			{
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual bool Equals(SongName another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SongName);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"name '{Value}' for {Song}";
		}
	}
}
