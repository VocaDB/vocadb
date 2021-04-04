#nullable disable

using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumPictureFile : EntryPictureFile, IEntryWithIntId
	{
		private Album _album;

		public AlbumPictureFile() { }

		public AlbumPictureFile(string name, string mime, User author, Album album)
			: base(name, mime, author)
		{
			_album = album;
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

		public override EntryType EntryType => EntryType.Album;

		public override int OwnerEntryId => Album.Id;

#nullable enable
		public virtual void Move(Album target)
		{
			ParamIs.NotNull(() => target);

			if (target == Album)
				return;

			Album.Pictures.Remove(this);
			Album = target;
			target.Pictures.Add(this);
		}
#nullable disable
	}
}
