using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Data contract for <see cref="FavoriteSongForUser"/>.
	/// SECURITY NOTE: user contract contains email, which is sensitive information.
	/// </summary>
	public class FavoriteSongForUserContract {

		public FavoriteSongForUserContract(FavoriteSongForUser favoriteSongForUser, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => favoriteSongForUser);

			Id = favoriteSongForUser.Id;
			Rating = favoriteSongForUser.Rating;
			Song = new SongContract(favoriteSongForUser.Song, languagePreference);
			User = new UserWithEmailContract(favoriteSongForUser.User);

		}

		public int Id { get; set; }

		public SongVoteRating Rating { get; set; }

		public SongContract Song { get; set; }

		public UserWithEmailContract User { get; set; }

	}

}
