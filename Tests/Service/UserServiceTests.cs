#nullable disable

using System.Linq;
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

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(_user.Name, result.Name, "Name");
		}

		[TestMethod]
		public void CheckAccessWithKey_Invalid()
		{
			var result = _service.CheckAccessWithKey(_user.Name, LoginManager.GetHashedAccessKey("rinrin"), "localhatsune", false);

			Assert.IsNull(result, "result");
		}

		[TestMethod]
		public void UpdateSongRating_Create()
		{
			var song = _repository.Save(CreateEntry.Song());
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Favorite);

			Assert.IsTrue(_user.FavoriteSongs.Any(s => s.Song == song), "Song was added to user");
			Assert.AreEqual(1, _user.FavoriteSongs.Count, "Number of favorite songs for user");
			Assert.AreEqual(1, _repository.List<FavoriteSongForUser>().Count, "Number of links in repo");
		}

		[TestMethod]
		public void UpdateSongRating_Update()
		{
			var song = _repository.Save(CreateEntry.Song());
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Favorite);
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Like);

			var rating = _user.FavoriteSongs.First(s => s.Song == song);
			Assert.AreEqual(SongVoteRating.Like, rating.Rating, "Rating");
			Assert.AreEqual(1, _user.FavoriteSongs.Count, "Number of favorite songs for user");
			Assert.AreEqual(1, _repository.List<FavoriteSongForUser>().Count, "Number of links in repo");
		}

		[TestMethod]
		public void UpdateSongRating_Delete()
		{
			var song = _repository.Save(CreateEntry.Song());
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Favorite);
			_service.UpdateSongRating(_user.Id, song.Id, SongVoteRating.Nothing);

			Assert.IsFalse(_user.FavoriteSongs.Any(s => s.Song == song), "Song was removed from user");
			Assert.AreEqual(0, _user.FavoriteSongs.Count, "Number of favorite songs for user");
			Assert.AreEqual(0, _repository.List<FavoriteSongForUser>().Count, "Number of links in repo");
		}
	}
}
