using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumPictureFile : EntryPictureFile {

		private Album album;

		public AlbumPictureFile() { }

		public AlbumPictureFile(string name, string mime, User author, Album album) 
			: base(name, mime, author) {

			this.album = album;

		}

		public virtual Album Album {
			get { return album; }
			set { 
				ParamIs.NotNull(() => value);
				album = value; 
			}
		}

		public override EntryType EntryType {
			get { return EntryType.Album; }
		}

		public virtual void Move(Album target) {

			ParamIs.NotNull(() => target);

			if (target == Album)
				return;

			Album.Pictures.Remove(this);
			Album = target;
			target.Pictures.Add(this);

		}

	}

}
