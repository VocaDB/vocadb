#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Users
{
	/// <summary>
	/// Tests for <see cref="FavoriteSongForUser"/>.
	/// </summary>
	[TestClass]
	public class FavoriteSongForUserTests
	{
		private FavoriteSongForUser _rating;
		private Song _song;
		private User _user;

		[TestInitialize]
		public void SetUp()
		{
			_song = new Song(new LocalizedString("I just wanna say...", ContentLanguageSelection.English));
			_user = new User { Name = "Miku" };
			_rating = _user.AddSongToFavorites(_song, SongVoteRating.Like);
		}

		[TestMethod]
		public void Delete()
		{
			_rating.Delete();

			Assert.IsFalse(_song.IsFavoritedBy(_user), "not favorited by user");
			Assert.AreEqual(0, _song.RatingScore, "rating score is updated");
		}

		[TestMethod]
		public void SetRating_Changed()
		{
			_rating.SetRating(SongVoteRating.Favorite);

			Assert.AreEqual(1, _song.FavoritedTimes, "1 favorite");
			Assert.AreEqual(FavoriteSongForUser.GetRatingScore(SongVoteRating.Favorite), _song.RatingScore, "rating score is updated");
		}
	}
}
