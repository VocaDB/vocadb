namespace VocaDb.Model.Domain.Songs {

	public class SongHit {

		private int agent;
		private Song song;

		public SongHit() { }

		public SongHit(Song song, int agent) {
			Agent = agent;
			Song = song;
		}

		public virtual int Agent {
			get { return agent; }
			set { agent = value; }
		}

		public virtual long Id { get; set; }

		public virtual Song Song {
			get { return song; }
			set { 
				ParamIs.NotNull(() => value);
				song = value; 
			}
		}

	}

}
