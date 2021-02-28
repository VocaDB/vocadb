#nullable disable

using System;
using System.Linq;
using FluentAssertions;
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

			result.Should().NotBeNull("result");
			result.Artist.Should().Be(artist, "Artist");
		}

		[TestMethod]
		public void AddOwnedArtist_AlreadyAdded()
		{
			var artist = new Artist { Id = 1 };

			_user.AddOwnedArtist(artist);
			_user.Invoking(subject => subject.AddOwnedArtist(artist)).Should().Throw<ArgumentException>();
		}

		[TestMethod]
		public void AddSongToFavorites_Like()
		{
			var rating = _user.AddSongToFavorites(_song, SongVoteRating.Like);

			rating.Should().NotBeNull("result is not null");
			rating.Rating.Should().Be(SongVoteRating.Like, "rating is as expected");
			_song.FavoritedTimes.Should().Be(0, "not favorited");
			_song.RatingScore.Should().Be(FavoriteSongForUser.GetRatingScore(SongVoteRating.Like), "rating score");
			_song.IsFavoritedBy(_user).Should().BeTrue("song is favorited by user");
		}

		[TestMethod]
		public void AddSongToFavorites_Favorite()
		{
			var rating = _user.AddSongToFavorites(_song, SongVoteRating.Favorite);

			rating.Should().NotBeNull("result is not null");
			rating.Rating.Should().Be(SongVoteRating.Favorite, "rating is as expected");
			_song.FavoritedTimes.Should().Be(1, "favorited once");
			_song.RatingScore.Should().Be(FavoriteSongForUser.GetRatingScore(SongVoteRating.Favorite), "rating score");
			_song.IsFavoritedBy(_user).Should().BeTrue("song is favorited by user");
		}

		[TestMethod]
		public void CreateWebLink()
		{
			_user.CreateWebLink(new WebLinkContract("http://www.test.com", "test link", WebLinkCategory.Other, disabled: false));

			_user.WebLinks.Count.Should().Be(1, "Should have one link");
			var link = _user.WebLinks.First();
			link.Description.Should().Be("test link", "description");
			link.Url.Should().Be("http://www.test.com", "url");
			link.Category.Should().Be(WebLinkCategory.Other, "category");
		}
	}
}
