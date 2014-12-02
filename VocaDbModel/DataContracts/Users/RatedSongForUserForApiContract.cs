using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class RatedSongForUserForApiContract {

		public RatedSongForUserForApiContract() { }

		public RatedSongForUserForApiContract(FavoriteSongForUser ratedSong, ContentLanguagePreference languagePreference, SongOptionalFields fields) {
			
			this.Rating = ratedSong.Rating;
			this.Song = new SongForApiContract(ratedSong.Song, null, languagePreference, fields);

		}

		[DataMember]
		public SongVoteRating Rating { get; set; }

		[DataMember]
		public SongForApiContract Song { get; set; }

	}

}
