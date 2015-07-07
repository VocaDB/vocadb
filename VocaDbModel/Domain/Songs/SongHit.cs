
namespace VocaDb.Model.Domain.Songs {

	public class SongHit : EntryHit {

		private Song song;

		public SongHit() { }

		public SongHit(Song song, int agent) 
			: base(agent) {
			Song = song;
		}

		public virtual Song Song {
			get { return song; }
			set { 
				ParamIs.NotNull(() => value);
				song = value; 
			}
		}

	}

}
