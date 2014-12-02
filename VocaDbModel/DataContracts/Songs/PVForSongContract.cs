using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class PVForSongContract : PVContract {

		public PVForSongContract(PVForSong pv, ContentLanguagePreference languagePreference)
			: base(pv) {

			Song = new SongContract(pv.Song, languagePreference);

		}

		public SongContract Song { get; set; }

	}

}
