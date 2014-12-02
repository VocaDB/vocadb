using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongWithComponentsContract : SongContract {

		public SongWithComponentsContract() {}

		public SongWithComponentsContract(Song song, ContentLanguagePreference languagePreference, bool includeArtists = false)
			: base(song, languagePreference) {

			if (includeArtists)
				Artists = song.ArtistList.Select(a => new ArtistContract(a, languagePreference)).ToArray();

		}

		[DataMember]
		public ArtistContract[] Artists { get; set; }

	}

}
