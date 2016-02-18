using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	public class CreateSongContract {

		public ArtistContract[] Artists { get; set; }

		public bool Draft { get; set; }

		public LocalizedStringContract[] Names { get; set; }

		public SongContract OriginalVersion { get; set; }

		public string PVUrl { get; set; }

		public string ReprintPVUrl { get; set; }

		public SongType SongType { get; set; }
		
	}

}
