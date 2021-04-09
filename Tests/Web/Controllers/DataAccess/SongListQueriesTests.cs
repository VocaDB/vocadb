#nullable disable

using System.IO;
using System.Linq;
using System.Net.Mime;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
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
			_permissionContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(_userWithSongList, ContentLanguagePreference.Default));

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
			songList.Should().NotBeNull("List was saved to repository");

			songList.Name.Should().Be(_songListContract.Name, "Name");
			songList.Description.Should().Be(_songListContract.Description, "Description");
			songList.AllSongs.Count.Should().Be(2, "Number of songs");
			songList.AllSongs[0].Song.DefaultName.Should().Be("Project Diva desu.", "First song as expected");
			songList.AllSongs[1].Song.DefaultName.Should().Be("World is Mine", "Second song as expected");
		}

		[TestMethod]
		public void Delete()
		{
			var list = _repository.Save(new SongList("Mikulist", _userWithSongList));
			var archived = _repository.Save(list.CreateArchivedVersion(new SongListDiff(), new AgentLoginData(_userWithSongList), EntryEditEvent.Created, string.Empty));
			_repository.Save(new SongListActivityEntry(list, EntryEditEvent.Created, _userWithSongList, archived)); // Note: activity entries are generally only created for featured song lists.

			_queries.MoveToTrash(list.Id);

			_repository.Count<SongList>().Should().Be(0, "Song list was removed");
			_repository.Count<ArchivedSongListVersion>().Should().Be(0, "Song list archived version was removed");
			_repository.Count<SongListActivityEntry>().Should().Be(0, "Activity entry was deleted");
		}

		[TestMethod]
		public void GetSongsInList_ByName()
		{
			var list = _repository.Save(new SongList("Mikulist", _userWithSongList));
			_repository.Save(list.AddSong(_song1));
			_repository.Save(list.AddSong(_song2));

			var result = _queries.GetSongsInList(new SongInListQueryParams { ListId = list.Id, TextQuery = SearchTextQuery.Create("Diva") });

			result.Items.Length.Should().Be(1);
			result.Items[0].Song.Name.Should().Be(_song1.DefaultName);
		}

		[TestMethod]
		public void GetSongsInList_ByDescription()
		{
			var list = _repository.Save(new SongList("Mikulist", _userWithSongList));
			_repository.Save(list.AddSong(_song1, 1, "encore"));
			_repository.Save(list.AddSong(_song2, 2, "encore"));

			var result = _queries.GetSongsInList(new SongInListQueryParams { ListId = list.Id, TextQuery = SearchTextQuery.Create("enc") });

			result.Items.Length.Should().Be(2);
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
			songList.AllSongs.Count.Should().Be(3, "Number of songs");
			songList.AllSongs[2].Song.DefaultName.Should().Be("Electric Angel", "New song as expected");
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
			_imagePersister.HasImage(thumb, ImageSize.Original).Should().BeTrue("Original image was saved");
			_imagePersister.HasImage(thumb, ImageSize.SmallThumb).Should().BeTrue("Thumbnail was saved");
		}
	}
}
