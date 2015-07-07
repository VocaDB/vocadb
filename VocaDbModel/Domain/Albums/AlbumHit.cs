
namespace VocaDb.Model.Domain.Albums {

	public class AlbumHit : EntryHit {

		private Album album;

		public AlbumHit() { }

		public AlbumHit(Album album, int agent) 
			: base(agent) {
			Album = album;
		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

	}

}
