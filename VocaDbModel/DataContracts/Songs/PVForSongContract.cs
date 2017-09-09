using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PVForSongContract : PVContract {

		public PVForSongContract(PVForSong pv, ContentLanguagePreference languagePreference)
			: base(pv) {

			Song = new SongContract(pv.Song, languagePreference);

		}

		[DataMember]
		public SongContract Song { get; set; }

	}

}
