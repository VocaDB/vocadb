using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Users {

	/// <summary>
	/// Tests for <see cref="FavoriteSongForUser"/>.
	/// </summary>
	[TestClass]
	public class FavoriteSongForUserTests {

		private FavoriteSongForUser rating;
		private Song song;
		private User user;

		[TestInitialize]
		public void SetUp() {

			song = new Song(new LocalizedString("I just wanna say...", ContentLanguageSelection.English));
			user = new User { Name = "Miku" };
			rating = user.AddSongToFavorites(song, SongVoteRating.Like);

		}

		[TestMethod]
		public void Delete() {
			
			rating.Delete();

			Assert.IsFalse(song.IsFavoritedBy(user), "not favorited by user");
			Assert.AreEqual(0, song.RatingScore, "rating score is updated");
			
		}

		[TestMethod]
		public void SetRating_Changed() {

			rating.SetRating(SongVoteRating.Favorite);

			Assert.AreEqual(1, song.FavoritedTimes, "1 favorite");
			Assert.AreEqual(FavoriteSongForUser.GetRatingScore(SongVoteRating.Favorite), song.RatingScore, "rating score is updated");

		}

	}
}
