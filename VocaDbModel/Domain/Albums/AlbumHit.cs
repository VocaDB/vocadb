namespace VocaDb.Model.Domain.Albums {

	public class AlbumHit {

		private int agent;
		private Album album;

		public AlbumHit() { }

		public AlbumHit(Album album, int agent) {
			Agent = agent;
			Album = album;
		}

		public virtual int Agent {
			get { return agent; }
			set { agent = value; }
		}

		public virtual long Id { get; set; }

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

	}

}
