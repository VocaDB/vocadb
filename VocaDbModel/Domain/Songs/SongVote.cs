using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class SongVote {

		private string hostname;
		private Song song;

		public virtual string Hostname {
			get { return hostname; }
			set { 
				ParamIs.NotNullOrEmpty(() => value);
				hostname = value; 
			}
		}

		public virtual SongVoteRating Rating { get; set; }

		public virtual Song Song {
			get { return song; }
			set { 
				ParamIs.NotNull(() => value);
				song = value; 
			}
		}

		public virtual User User { get; set; }

	}
}
