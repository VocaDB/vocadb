#nullable disable

using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="SongListQueries"/>.
	/// </summary>
	[TestClass]
	public class SongListQueriesTests
	{
		private InMemoryImagePersister _imagePersister;
		private FakePermissionContext _permissionContext;
		private FakeSongListRepository _repository;
		private SongListForEditContract _songListContract;
		private SongListQueries _queries;
		private Song _song1;
		private Song _song2;
		private User _userWithSongList;

		private SongInListEditContract[] SongInListEditContracts(params Song[] songs)
		{
			return songs.Select(s => new SongInListEditContract(new SongInList(s, new SongList(), 0, string.Empty), ContentLanguagePreference.Default)).ToArray();
		}

		private Stream TestImage()
		{
			return ResourceHelper.GetFileStream("yokohma_bay_concert.jpg");
		}

		[TestInitialize]
		public void SetUp()
		{
			_repository = new FakeSongListRepository();
			_userWithSongList = new User("User with songlist", "123", "test@test.com", PasswordHashAlgorithms.Default);
			_permissionContext = new FakePermissionContext(new UserWithPermissionsContract(_userWithSongList, ContentLanguagePreference.Default));

			_imagePersister = new InMemoryImagePersister();
			_queries = new SongListQueries(_repository, _permissionContext, new FakeEntryLinkFactory(), _imagePersister, _imagePersister, new FakeUserIconFactory());

			_song1 = new Song(TranslatedString.Create("Project Diva desu.")) { Id = 1 };
			_song2 = new Song(TranslatedString.Create("World is Mine")) { Id = 2 };

			_repository.Add(_userWithSongList);
			_repository.Add(_song1, _song2);

			_songListContract = new SongListForEditContract
			{
				Name = "Mikunopolis Setlist",
				Description = "MIKUNOPOLIS in LOS ANGELES - Hatsune Miku US debut concert held at Nokia Theatre for Anime Expo 2011 on 2nd July 2011.",
				SongLinks = SongInListEditContracts(_song1, _song2)
			};
		}

		[TestMethod]
		public void Create()
		{
			_queries.UpdateSongList(_songListContract, null);

			var songList = _repository.List<SongList>().FirstOrDefault();
			Assert.IsNotNull(songList, "List was saved to repository");

			Assert.AreEqual(_songListContract.Name, songList.Name, "Name");
			Assert.AreEqual(_songListContract.Description, songList.Description, "Description");
			Assert.AreEqual(2, songList.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Project Diva desu.", songList.AllSongs[0].Song.DefaultName, "First song as expected");
			Assert.AreEqual("World is Mine", songList.AllSongs[1].Song.DefaultName, "Second song as expected");
		}

		[TestMethod]
		public void Delete()
		{
			var list = _repository.Save(new SongList("Mikulist", _userWithSongList));
			var archived = _repository.Save(list.CreateArchivedVersion(new SongListDiff(), new AgentLoginData(_userWithSongList), EntryEditEvent.Created, string.Empty));
			_repository.Save(new SongListActivityEntry(list, EntryEditEvent.Created, _userWithSongList, archived)); // Note: activity entries are generally only created for featured song lists.

			_queries.MoveToTrash(list.Id);

			Assert.AreEqual(0, _repository.Count<SongList>(), "Song list was removed");
			Assert.AreEqual(0, _repository.Count<ArchivedSongListVersion>(), "Song list archived version was removed");
			Assert.AreEqual(0, _repository.Count<SongListActivityEntry>(), "Activity entry was deleted");
		}

		[TestMethod]
		public void GetSongsInList_ByName()
		{
			var list = _repository.Save(new SongList("Mikulist", _userWithSongList));
			_repository.Save(list.AddSong(_song1));
			_repository.Save(list.AddSong(_song2));

			var result = _queries.GetSongsInList(new SongInListQueryParams { ListId = list.Id, TextQuery = SearchTextQuery.Create("Diva") });

			Assert.AreEqual(1, result.Items.Length);
			Assert.AreEqual(_song1.DefaultName, result.Items[0].Song.Name);
		}

		[TestMethod]
		public void GetSongsInList_ByDescription()
		{
			var list = _repository.Save(new SongList("Mikulist", _userWithSongList));
			_repository.Save(list.AddSong(_song1, 1, "encore"));
			_repository.Save(list.AddSong(_song2, 2, "encore"));

			var result = _queries.GetSongsInList(new SongInListQueryParams { ListId = list.Id, TextQuery = SearchTextQuery.Create("enc") });

			Assert.AreEqual(2, result.Items.Length);
		}

		[TestMethod]
		public void UpdateSongLinks()
		{
			// Create list
			_songListContract.Id = _queries.UpdateSongList(_songListContract, null);

			var newSong = new Song(TranslatedString.Create("Electric Angel"));
			_repository.Save(newSong);

			_songListContract.SongLinks = _songListContract.SongLinks.Concat(SongInListEditContracts(newSong)).ToArray();

			// Update list
			_queries.UpdateSongList(_songListContract, null);

			var songList = _repository.List<SongList>().First();
			Assert.AreEqual(3, songList.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Electric Angel", songList.AllSongs[2].Song.DefaultName, "New song as expected");
		}

		[TestMethod]
		public void Update_Image()
		{
			int id;
			using (var stream = TestImage())
			{
				id = _queries.UpdateSongList(_songListContract, new UploadedFileContract { Mime = MediaTypeNames.Image.Jpeg, Stream = stream });
			}

			var songList = _repository.Load(id);

			var thumb = new EntryThumb(songList, MediaTypeNames.Image.Jpeg, ImagePurpose.Main);
			Assert.IsTrue(_imagePersister.HasImage(thumb, ImageSize.Original), "Original image was saved");
			Assert.IsTrue(_imagePersister.HasImage(thumb, ImageSize.SmallThumb), "Thumbnail was saved");
		}
	}
}
