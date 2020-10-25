using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song {

	public class SongIconLinkViewModel {

		public SongIconLinkViewModel(SongForApiContract song) {
			Song = song;
		}

		public SongForApiContract Song { get; set; }

	}

}