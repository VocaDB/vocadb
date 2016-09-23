using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.UseCases {

	public class UsersWithSongRatingContract {

		public UsersWithSongRatingContract() { }

		public UsersWithSongRatingContract(IList<FavoriteSongForUser> favorites, ContentLanguagePreference languagePreference) {

			var knownRatings = favorites
				.Where(a => a.User.Options.PublicRatings)
				.OrderBy(u => u.User.Name)
				.Select(u => new FavoriteSongForUserContract(u, languagePreference))
				.ToArray();

			FavoritedBy = knownRatings.Where(r => r.Rating == SongVoteRating.Favorite).Select(u => u.User).ToArray();
			LikedBy = knownRatings.Where(r => r.Rating == SongVoteRating.Like).Select(u => u.User).ToArray();
			HiddenRatingCount = favorites.Count(r => !r.User.Options.PublicRatings);

		}

		public UserWithEmailContract[] FavoritedBy { get; set; }

		public int HiddenRatingCount { get; set; }

		public UserWithEmailContract[] LikedBy { get; set; }

	}

}
