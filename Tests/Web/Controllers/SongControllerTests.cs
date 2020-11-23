using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
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

		private readonly SongController controller;
		private readonly FakePermissionContext permissionContext = new FakePermissionContext();
		private readonly FakeSongRepository repository = new FakeSongRepository();

		public SongControllerTests()
		{
			permissionContext.SetLoggedUser(repository.Save(CreateEntry.User()));
			var queries = new SongQueries(repository, permissionContext, new FakeEntryLinkFactory(),
				new FakePVParser(), new FakeUserMessageMailer(), new FakeLanguageDetector(), new FakeUserIconFactory(), new EnumTranslations(), new InMemoryImagePersister(),
				new FakeObjectCache(), new VdbConfigManager(), new EntrySubTypeNameFactory(), new FakeFollowedArtistNotifier());
			controller = new SongController(null, queries, null, null);
		}

		[TestMethod]
		public async Task Create()
		{

			var artist = repository.Save(CreateEntry.Artist(ArtistType.Producer));

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

			var result = await controller.Create(model);
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "result");
			var routeResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Edit", routeResult.RouteValues["Action"], "Action");

			Assert.AreEqual(1, repository.List<Song>().Count, "Song was created");

		}

	}

}
