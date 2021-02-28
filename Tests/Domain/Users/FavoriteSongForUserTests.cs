#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
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

			_song.IsFavoritedBy(_user).Should().BeFalse("not favorited by user");
			_song.RatingScore.Should().Be(0, "rating score is updated");
		}

		[TestMethod]
		public void SetRating_Changed()
		{
			_rating.SetRating(SongVoteRating.Favorite);

			_song.FavoritedTimes.Should().Be(1, "1 favorite");
			_song.RatingScore.Should().Be(FavoriteSongForUser.GetRatingScore(SongVoteRating.Favorite), "rating score is updated");
		}
	}
}
