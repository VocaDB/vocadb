using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="SongListQueries"/>.
	/// </summary>
	[TestClass]
	public class SongListQueriesTests {

		private InMemoryImagePersister imagePersister;
		private FakePermissionContext permissionContext;
		private FakeSongListRepository repository;
		private SongListForEditContract songListContract;
		private SongListQueries queries;
		private User userWithSongList;

		private SongInListEditContract[] SongInListEditContracts(params Song[] songs) {
			return songs.Select(s => new SongInListEditContract(new SongInList(s, new SongList(), 0, string.Empty), ContentLanguagePreference.Default)).ToArray();
		}

		private Stream TestImage() {
			return ResourceHelper.GetFileStream("yokohma_bay_concert.jpg");
		}

		[TestInitialize]
		public void SetUp() {
			
			repository = new FakeSongListRepository();
			userWithSongList = new User("User with songlist", "123", "test@test.com", 123);
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(userWithSongList, ContentLanguagePreference.Default));

			imagePersister = new InMemoryImagePersister();
			queries = new SongListQueries(repository, permissionContext, new FakeEntryLinkFactory(), imagePersister);

			var song1 = new Song(TranslatedString.Create("Project Diva desu.")) { Id = 1};
			var song2 = new Song(TranslatedString.Create("World is Mine")) { Id = 2};

			repository.Add(userWithSongList);
			repository.Add(song1, song2);

			songListContract = new SongListForEditContract {
				Name = "Mikunopolis Setlist",
				Description = "MIKUNOPOLIS in LOS ANGELES - Hatsune Miku US debut concert held at Nokia Theatre for Anime Expo 2011 on 2nd July 2011.",
				SongLinks = SongInListEditContracts(song1, song2)
			};

		}

		[TestMethod]
		public void Create() {

			queries.UpdateSongList(songListContract, null);

			var songList = repository.List<SongList>().FirstOrDefault();
			Assert.IsNotNull(songList, "List was saved to repository");

			Assert.AreEqual(songListContract.Name, songList.Name, "Name");
			Assert.AreEqual(songListContract.Description, songList.Description, "Description");
			Assert.AreEqual(2, songList.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Project Diva desu.", songList.AllSongs[0].Song.DefaultName, "First song as expected");
			Assert.AreEqual("World is Mine", songList.AllSongs[1].Song.DefaultName, "Second song as expected");

		}

		[TestMethod]
		public void UpdateSongLinks() {

			// Create list
			songListContract.Id = queries.UpdateSongList(songListContract, null);

			var newSong = new Song(TranslatedString.Create("Electric Angel"));
			repository.Save(newSong);

			songListContract.SongLinks = songListContract.SongLinks.Concat(SongInListEditContracts(newSong)).ToArray();

			// Update list
			queries.UpdateSongList(songListContract, null);

			var songList = repository.List<SongList>().First();
			Assert.AreEqual(3, songList.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Electric Angel", songList.AllSongs[2].Song.DefaultName, "New song as expected");

		}

		[TestMethod]
		public void Update_Image() {
			
			int id;
			using (var stream = TestImage()) {
				id = queries.UpdateSongList(songListContract, new UploadedFileContract { Mime = MediaTypeNames.Image.Jpeg, Stream = stream });			
			}

			var songList = repository.Load(id);

			var thumb = new EntryThumb(songList, MediaTypeNames.Image.Jpeg);
			Assert.IsTrue(imagePersister.HasImage(thumb, ImageSize.Original), "Original image was saved");
			Assert.IsTrue(imagePersister.HasImage(thumb, ImageSize.SmallThumb), "Thumbnail was saved");

		}
	}

}
