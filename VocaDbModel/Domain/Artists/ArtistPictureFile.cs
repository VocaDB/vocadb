using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Artists
{

	public class ArtistPictureFile : EntryPictureFile, IEntryWithIntId
	{

		private Artist artist;

		public ArtistPictureFile() { }

		public ArtistPictureFile(string name, string mime, User author, Artist artist)
			: base(name, mime, author)
		{

			this.artist = artist;

		}

		public virtual Artist Artist
		{
			get { return artist; }
			set
			{
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public override EntryType EntryType
		{
			get { return EntryType.Artist; }
		}

		public override int OwnerEntryId
		{
			get { return Artist.Id; }
		}

		public virtual void Move(Artist target)
		{

			ParamIs.NotNull(() => target);

			if (target == Artist)
				return;

			Artist.Pictures.Remove(this);
			Artist = target;
			target.Pictures.Add(this);

		}

	}

}
