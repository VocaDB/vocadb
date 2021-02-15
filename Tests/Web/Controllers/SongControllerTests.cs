#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Song;

namespace VocaDb.Tests.Web.Controllers
{
	/// <summary>
	/// Tests for <see cref="SongController"/>.
	/// </summary>
	[TestClass]
	public class SongControllerTests
	{
		private readonly SongController _controller;
		private readonly FakePermissionContext _permissionContext = new();
		private readonly FakeSongRepository _repository = new();

		public SongControllerTests()
		{
			_permissionContext.SetLoggedUser(_repository.Save(CreateEntry.User()));
			var queries = new SongQueries(_repository, _permissionContext, new FakeEntryLinkFactory(),
				new FakePVParser(), new FakeUserMessageMailer(), new FakeLanguageDetector(), new FakeUserIconFactory(), new EnumTranslations(), new InMemoryImagePersister(),
				new FakeObjectCache(), new VdbConfigManager(), new EntrySubTypeNameFactory(), new FakeFollowedArtistNotifier());
			_controller = new SongController(null, queries, null, null, null);
		}

		[TestMethod]
		public async Task Create()
		{
			var artist = _repository.Save(CreateEntry.Artist(ArtistType.Producer));

			var model = new Create
			{
				NameOriginal = "Arabian Response",
				PVUrl = "http://www.nicovideo.jp/watch/sm32982184",
				Artists = new[] {
					new ArtistForSongContract {
						Artist = new ArtistContract(artist, ContentLanguagePreference.Default)
					}
				}
			};

			var result = await _controller.Create(model);
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "result");
			var routeResult = (RedirectToActionResult)result;
			Assert.AreEqual("Edit", routeResult.ActionName, "Action");

			Assert.AreEqual(1, _repository.List<Song>().Count, "Song was created");
		}
	}
}
