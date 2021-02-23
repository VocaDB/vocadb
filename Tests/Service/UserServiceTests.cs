#nullable disable

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service
{
	[TestClass]
	public class UserServiceTests
	{
		private FakeUserRepository _repository;
		private UserService _service;
		private User _user;

		[TestInitialize]
		public void SetUp()
		{
			_user = CreateEntry.User();
			_repository = new FakeUserRepository(_user);
			_service = new UserService(_repository, new FakePermissionContext(_user), new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new FakeUserIconFactory());
		}

		[TestMethod]
		public void CheckAccessWithKey_Valid()
		{
			var result = _service.CheckAccessWithKey(_user.Name, LoginManager.GetHashedAccessKey(_user.AccessKey), "localhatsune", false);

			result.Should().NotBeNull("result");
			result.Name.Should().Be(_user.Name, "Name");
		}

		[TestMethod]
		public void CheckAccessWithKey_Invalid()
		{
			var result = _service.CheckAccessWithKey(_user.Name, LoginManager.GetHashedAccessKey("rinrin"), "localhatsune", false);

			result.Should().BeNull("result");
		}

		[TestMethod]
		public void UpdateSongRating_Create()
		{
			var song = _repository.Save(CreateEntry.Song());
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Favorite);

			_user.FavoriteSongs.Any(s => s.Song == song).Should().BeTrue("Song was added to user");
			_user.FavoriteSongs.Count.Should().Be(1, "Number of favorite songs for user");
			_repository.List<FavoriteSongForUser>().Count.Should().Be(1, "Number of links in repo");
		}

		[TestMethod]
		public void UpdateSongRating_Update()
		{
			var song = _repository.Save(CreateEntry.Song());
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Favorite);
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Like);

			var rating = _user.FavoriteSongs.First(s => s.Song == song);
			rating.Rating.Should().Be(SongVoteRating.Like, "Rating");
			_user.FavoriteSongs.Count.Should().Be(1, "Number of favorite songs for user");
			_repository.List<FavoriteSongForUser>().Count.Should().Be(1, "Number of links in repo");
		}

		[TestMethod]
		public void UpdateSongRating_Delete()
		{
			var song = _repository.Save(CreateEntry.Song());
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Favorite);
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Nothing);

			_user.FavoriteSongs.Any(s => s.Song == song).Should().BeFalse("Song was removed from user");
			_user.FavoriteSongs.Count.Should().Be(0, "Number of favorite songs for user");
			_repository.List<FavoriteSongForUser>().Count.Should().Be(0, "Number of links in repo");
		}
	}
}
