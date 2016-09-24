using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class RatedSongForUserForApiContract {

		public RatedSongForUserForApiContract() { }

		public RatedSongForUserForApiContract(FavoriteSongForUser ratedSong, IUserIconFactory userIconFactory, UserOptionalFields userFields) {

			this.Rating = ratedSong.Rating;
			if (ratedSong.User.Options.PublicRatings) {
				User = new UserForApiContract(ratedSong.User, userIconFactory, userFields);
			}

		}

		public RatedSongForUserForApiContract(FavoriteSongForUser ratedSong, ContentLanguagePreference languagePreference, SongOptionalFields fields) {
			
			this.Rating = ratedSong.Rating;
			this.Song = new SongForApiContract(ratedSong.Song, null, languagePreference, fields);

		}

		[DataMember]
		public SongVoteRating Rating { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SongForApiContract Song { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public UserForApiContract User { get; set; }

	}

}
