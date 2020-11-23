namespace VocaDb.Model.Domain.Albums
{
	public class AlbumIdentifier : IEntryWithIntId
	{
		private Album album;
		private string value;

		public AlbumIdentifier() { }

		public AlbumIdentifier(Album album, string value)
		{
			Album = album;
			Value = value;
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

		public virtual int Id { get; set; }

		public virtual string Value
		{
			get { return value; }
			set
			{
				ParamIs.NotNull(() => value);
				this.value = value;
			}
		}

		public virtual bool ContentEquals(AlbumIdentifier another)
		{
			return string.Equals(Value, another.Value);
		}
	}
}
