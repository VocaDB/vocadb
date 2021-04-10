#nullable disable

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumIdentifier : IEntryWithIntId
	{
		private Album _album;
		private string _value;

		public AlbumIdentifier() { }

		public AlbumIdentifier(Album album, string value)
		{
			Album = album;
			Value = value;
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

		public virtual int Id { get; set; }

		public virtual string Value
		{
			get => _value;
			set
			{
				ParamIs.NotNull(() => value);
				_value = value;
			}
		}

#nullable enable
		public virtual bool ContentEquals(AlbumIdentifier another)
		{
			return string.Equals(Value, another.Value);
		}
#nullable disable
	}
}
