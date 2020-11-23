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
		private FakeUserRepository repository;
		private UserService service;
		private User user;

		[TestInitialize]
		public void SetUp()
		{
			user = CreateEntry.User();
			repository = new FakeUserRepository(user);
			service = new UserService(repository, new FakePermissionContext(user), new FakeEntryLinkFactory(), new FakeUserMessageMailer());
		}

		[TestMethod]
		public void CheckAccessWithKey_Valid()
		{
			var result = service.CheckAccessWithKey(user.Name, LoginManager.GetHashedAccessKey(user.AccessKey), "localhatsune", false);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(user.Name, result.Name, "Name");
		}

		[TestMethod]
		public void CheckAccessWithKey_Invalid()
		{
			var result = service.CheckAccessWithKey(user.Name, LoginManager.GetHashedAccessKey("rinrin"), "localhatsune", false);

			Assert.IsNull(result, "result");
		}

		[TestMethod]
		public void UpdateSongRating_Create()
		{
			var song = repository.Save(CreateEntry.Song());
			service.UpdateSongRating(user.Id, song.Id, SongVoteRating.Favorite);

			Assert.IsTrue(user.FavoriteSongs.Any(s => s.Song == song), "Song was added to user");
			Assert.AreEqual(1, user.FavoriteSongs.Count, "Number of favorite songs for user");
			Assert.AreEqual(1, repository.List<FavoriteSongForUser>().Count, "Number of links in repo");
		}

		[TestMethod]
		public void UpdateSongRating_Update()
		{
			var song = repository.Save(CreateEntry.Song());
			service.UpdateSongRating(user.Id, song.Id, SongVoteRating.Favorite);
			service.UpdateSongRating(user.Id, song.Id, SongVoteRating.Like);

			var rating = user.FavoriteSongs.First(s => s.Song == song);
			Assert.AreEqual(SongVoteRating.Like, rating.Rating, "Rating");
			Assert.AreEqual(1, user.FavoriteSongs.Count, "Number of favorite songs for user");
			Assert.AreEqual(1, repository.List<FavoriteSongForUser>().Count, "Number of links in repo");
		}

		[TestMethod]
		public void UpdateSongRating_Delete()
		{
			var song = repository.Save(CreateEntry.Song());
			service.UpdateSongRating(user.Id, song.Id, SongVoteRating.Favorite);
			service.UpdateSongRating(user.Id, song.Id, SongVoteRating.Nothing);

			Assert.IsFalse(user.FavoriteSongs.Any(s => s.Song == song), "Song was removed from user");
			Assert.AreEqual(0, user.FavoriteSongs.Count, "Number of favorite songs for user");
			Assert.AreEqual(0, repository.List<FavoriteSongForUser>().Count, "Number of links in repo");
		}
	}
}
