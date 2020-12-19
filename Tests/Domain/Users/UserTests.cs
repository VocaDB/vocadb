#nullable disable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Users
{
	/// <summary>
	/// Tests for <see cref="User"/>.
	/// </summary>
	[TestClass]
	public class UserTests
	{
		private Song _song;
		private User _user;

		[TestInitialize]
		public void SetUp()
		{
			_user = new User();
			_song = new Song(new LocalizedString("I just wanna say...", ContentLanguageSelection.English));
		}

		[TestMethod]
		public void AddOwnedArtist_New()
		{
			var artist = new Artist { Id = 1 };

			var result = _user.AddOwnedArtist(artist);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(artist, result.Artist, "Artist");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddOwnedArtist_AlreadyAdded()
		{
			var artist = new Artist { Id = 1 };

			_user.AddOwnedArtist(artist);
			_user.AddOwnedArtist(artist);
		}

		[TestMethod]
		public void AddSongToFavorites_Like()
		{
			var rating = _user.AddSongToFavorites(_song, SongVoteRating.Like);

			Assert.IsNotNull(rating, "result is not null");
			Assert.AreEqual(SongVoteRating.Like, rating.Rating, "rating is as expected");
			Assert.AreEqual(0, _song.FavoritedTimes, "not favorited");
			Assert.AreEqual(FavoriteSongForUser.GetRatingScore(SongVoteRating.Like), _song.RatingScore, "rating score");
			Assert.IsTrue(_song.IsFavoritedBy(_user), "song is favorited by user");
		}

		[TestMethod]
		public void AddSongToFavorites_Favorite()
		{
			var rating = _user.AddSongToFavorites(_song, SongVoteRating.Favorite);

			Assert.IsNotNull(rating, "result is not null");
			Assert.AreEqual(SongVoteRating.Favorite, rating.Rating, "rating is as expected");
			Assert.AreEqual(1, _song.FavoritedTimes, "favorited once");
			Assert.AreEqual(FavoriteSongForUser.GetRatingScore(SongVoteRating.Favorite), _song.RatingScore, "rating score");
			Assert.IsTrue(_song.IsFavoritedBy(_user), "song is favorited by user");
		}

		[TestMethod]
		public void CreateWebLink()
		{
			_user.CreateWebLink(new WebLinkContract("http://www.test.com", "test link", WebLinkCategory.Other, disabled: false));

			Assert.AreEqual(1, _user.WebLinks.Count, "Should have one link");
			var link = _user.WebLinks.First();
			Assert.AreEqual("test link", link.Description, "description");
			Assert.AreEqual("http://www.test.com", link.Url, "url");
			Assert.AreEqual(WebLinkCategory.Other, link.Category, "category");
		}
	}
}
