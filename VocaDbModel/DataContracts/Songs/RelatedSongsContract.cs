using System.Linq;

namespace VocaDb.Model.DataContracts.Songs {

	public class RelatedSongsContract {

		public bool Any {
			get {
				return ArtistMatches.Any() || LikeMatches.Any() || TagMatches.Any();
			}
		}

		public SongContract[] ArtistMatches { get; set; }

		public SongContract[] LikeMatches { get; set; }

		public SongContract[] TagMatches { get; set; }

	}

}