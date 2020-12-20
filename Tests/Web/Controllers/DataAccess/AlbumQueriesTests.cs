#nullable disable

using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="AlbumQueries"/>.
	/// </summary>
	[TestClass]
	public class AlbumQueriesTests
	{
		private Album _album;
		private InMemoryImagePersister _imagePersister;
		private FakeUserMessageMailer _mailer;
		private CreateAlbumContract _newAlbumContract;
		private FakePermissionContext _permissionContext;
		private Artist _producer;
		private FakeAlbumRepository _repository;
		private AlbumQueries _queries;
		private Song _song;
		private Song _song2;
		private Song _song3;
		private User _user;
		private User _user2;
		private Artist _vocalist;
		private Artist _vocalist2;

		private void AssertHasArtist(Album album, string artistName, ArtistRoles? roles)
		{
			VocaDbAssert.HasArtist(album, artistName, roles);
		}

		private void AssertHasArtist(Album album, Artist artist, ArtistRoles? roles)
		{
			VocaDbAssert.HasArtist(album, artist, roles);
		}

		private CommentForApiContract CreateComment(int albumId, string message)
		{
			var contract = new CommentForApiContract { Message = message, Author = new UserForApiContract(_user, null, UserOptionalFields.None) };
			return _queries.CreateComment(albumId, contract);
		}

		private SongInAlbumEditContract CreateSongInAlbumEditContract(int trackNumber, int songId = 0, string songName = null)
		{
			return new SongInAlbumEditContract { DiscNumber = 1, TrackNumber = trackNumber, SongId = songId, SongName = songName, Artists = new ArtistContract[0] };
		}

		private ArtistForAlbumContract CreateArtistForAlbumContract(int artistId = 0, string customArtistName = null, ArtistRoles roles = ArtistRoles.Default)
		{
			if (artistId != 0)
				return new ArtistForAlbumContract { Artist = new ArtistContract { Id = artistId }, Roles = roles };
			else
				return new ArtistForAlbumContract { Name = customArtistName, Roles = roles };
		}

		private Task<AlbumForEditContract> CallUpdate(AlbumForEditContract contract)
		{
			return _queries.UpdateBasicProperties(contract, null);
		}

		private async Task<AlbumForEditContract> CallUpdate(Stream image)
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());
			using (var stream = image)
			{
				return await _queries.UpdateBasicProperties(contract, new EntryPictureFileContract(stream, MediaTypeNames.Image.Jpeg, purpose: ImagePurpose.Main));
			}
		}

		private void Save<T>(params T[] entity) where T : class, IDatabaseObject
		{
			_repository.Save(entity);
		}

		private T Save<T>(T entity) where T : class, IDatabaseObject
		{
			return _repository.Save(entity);
		}

		[TestInitialize]
		public void SetUp()
		{
			_producer = CreateEntry.Producer();
			_vocalist = CreateEntry.Vocalist(name: "Hatsune Miku");
			_vocalist2 = CreateEntry.Vocalist(name: "Rin");

			_album = CreateEntry.Album(id: 39, name: "Synthesis");
			_repository = new FakeAlbumRepository(_album);
			foreach (var name in _album.Names)
				Save(name);
			_user = CreateEntry.User(1, "Miku");
			_user.GroupId = UserGroupId.Moderator;
			_user2 = CreateEntry.User(2, "Luka");
			Save(_user, _user2);
			Save(_producer, _vocalist, _vocalist2);

			_song = Save(CreateEntry.Song(name: "Nebula"));
			_song2 = Save(CreateEntry.Song(name: "Anger"));
			_song3 = Save(CreateEntry.Song(name: "Resistance"));

			_permissionContext = new FakePermissionContext(_user);
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			_newAlbumContract = new CreateAlbumContract
			{
				DiscType = DiscType.Album,
				Names = new[] {
					new LocalizedStringContract("Another Dimensions", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistContract(_producer, ContentLanguagePreference.Default),
					new ArtistContract(_vocalist, ContentLanguagePreference.Default),
				}
			};

			_imagePersister = new InMemoryImagePersister();
			_mailer = new FakeUserMessageMailer();
			_queries = new AlbumQueries(_repository, _permissionContext, entryLinkFactory, _imagePersister, _imagePersister, _mailer,
				new FakeUserIconFactory(), new EnumTranslations(), new FakePVParser(),
				new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()), new InMemoryImagePersister(),
				new FakeObjectCache());
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await _queries.Create(_newAlbumContract);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Another Dimensions", result.Name, "Name");

			_album = _repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Another Dimensions"));

			Assert.IsNotNull(_album, "Album was saved to repository");
			Assert.AreEqual("Another Dimensions", _album.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, _album.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, _album.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(_album.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", _album.ArtistString.Default, "ArtistString");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(_album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(_album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public async Task Create_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			await _queries.Create(_newAlbumContract);
		}

		[TestMethod]
		public void CreateComment()
		{
			var result = CreateComment(39, "Hello world");
			Assert.IsNotNull(result, "Result");

			var comment = _repository.List<Comment>().FirstOrDefault();
			Assert.IsNotNull(comment, "Comment was saved");
			Assert.AreEqual(_user, comment.Author, "Author");
			Assert.AreEqual(_album, comment.Entry, "Album");
			Assert.AreEqual("Hello world", comment.Message, "Comment message");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void CreateComment_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			CreateComment(39, "Hello world");
		}

		[TestMethod]
		public async Task GetCoverPictureThumb()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());

			var result = _queries.GetCoverPictureThumb(contract.Id);

			Assert.IsNotNull(result, "result");
			Assert.IsNotNull(result.Picture, "Picture");
			Assert.IsNotNull(result.Picture.Bytes, "Picture content");
			Assert.AreEqual(contract.CoverPictureMime, result.Picture.Mime, "Picture MIME");
			Assert.AreEqual(contract.Id, result.EntryId, "EntryId");
		}

		[TestMethod]
		public void GetAlbumDetails()
		{
			_repository.Save(_album.AddSong(_song, 1, 1));
			_repository.Save(_album.AddSong(_song2, 2, 1));
			_repository.Save(_user.AddSongToFavorites(_song, SongVoteRating.Favorite));

			var result = _queries.GetAlbumDetails(_album.Id, "miku@vocaloid.eu");

			Assert.AreEqual(2, result.Songs.Length, "Number of songs");
			var track = result.Songs[0];
			Assert.AreEqual(SongVoteRating.Favorite, track.Rating, "First track rating");
			Assert.AreEqual(SongVoteRating.Nothing, result.Songs[1].Rating, "Second track rating");
		}

		[TestMethod]
		public void GetAlbumDetails_CustomTrack()
		{
			_repository.Save(_album.AddSong("Miku Miku", 1, 1));
			_repository.Save(_user.AddSongToFavorites(_song, SongVoteRating.Favorite));

			var result = _queries.GetAlbumDetails(_album.Id, "miku@vocaloid.eu");

			Assert.AreEqual(1, result.Songs.Length, "Number of songs");
		}

		[TestMethod]
		public void GetAlbumDetails_NotLoggedIn()
		{
			_repository.Save(_album.AddSong("Miku Miku", 1, 1));
			_permissionContext.LogOff();

			var result = _queries.GetAlbumDetails(_album.Id, "miku@vocaloid.eu");
			Assert.IsNotNull(result, "result");
		}

		[TestMethod]
		public async Task GetTagSuggestions()
		{
			void AddTagUsages(Song[] songs, string[] tagNames)
			{
				var (tags, tagUsages, tagVotes) = CreateEntry.TagUsages(songs, tagNames, _user, new SongTagUsageFactory(_repository.CreateContext(), _song));
				_repository.Save(tags);
				_repository.Save(tagUsages);
				_repository.Save(tagVotes);
			}

			AddTagUsages(new[] { _song, _song2 }, new[] { "vocarock", "techno" });

			_repository.Save(_album.AddSong(_song, 1, 1), _album.AddSong(_song2, 2, 1));

			var result = await _queries.GetTagSuggestions(_album.Id);

			Assert.AreEqual(2, result.Length, "Number of tag suggestions");
			Assert.IsTrue(result.Any(r => r.Tag.Name == "vocarock"), "First tag was returned");
		}

		[TestMethod]
		public void Merge_ToEmpty()
		{
			Save(_album.AddArtist(_producer));
			Save(_album.AddArtist(_vocalist));
			Save(_album.AddSong(_song, 1, 1));

			var album2 = CreateEntry.Album();
			Save(album2);

			_queries.Merge(_album.Id, album2.Id);

			Assert.IsTrue(_album.Deleted, "Original was deleted");
			Assert.AreEqual(0, _album.AllArtists.Count, "All artists removed from original");
			Assert.AreEqual(0, _album.AllSongs.Count, "All songs removed from original");

			Assert.AreEqual("Synthesis", album2.DefaultName, "Name");
			Assert.AreEqual(2, album2.AllArtists.Count, "Number of artists");
			Assert.AreEqual(1, album2.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Nebula", album2.AllSongs.First().Song.DefaultName);

			var mergeRecord = _repository.List<AlbumMergeRecord>().FirstOrDefault(m => m.Source == _album.Id);
			Assert.IsNotNull(mergeRecord, "Merge record was created");
			Assert.AreEqual(album2.Id, mergeRecord.Target.Id, "mergeRecord.Target.Id");
		}

		[TestMethod]
		public void Merge_WithArtists()
		{
			Save(_album.AddArtist(_producer, false, ArtistRoles.Instrumentalist));
			Save(_album.AddArtist(_vocalist, false, ArtistRoles.Default));

			var album2 = Save(CreateEntry.Album());
			Save(album2.AddArtist(_vocalist, false, ArtistRoles.Mastering));
			Save(album2.AddArtist("Kaito", true, ArtistRoles.Default));

			_queries.Merge(_album.Id, album2.Id);

			Assert.AreEqual(1, _album.AllArtists.Count, "Number of artists for source"); // Vocalist was not moved
			Assert.AreEqual(3, album2.AllArtists.Count, "Number of artists for target");
			AssertHasArtist(album2, _producer, ArtistRoles.Instrumentalist);
			AssertHasArtist(album2, _vocalist, ArtistRoles.Mastering);
			AssertHasArtist(album2, "Kaito", ArtistRoles.Default);
		}

		[TestMethod]
		public void Merge_WithTracks()
		{
			Save(_album.AddSong(_song, 1, 1));
			Save(_album.AddSong(_song2, 2, 1));

			var album2 = Save(CreateEntry.Album());
			Save(album2.AddSong(_song, 1, 1));
			Save(album2.AddSong(_song3, 2, 1));

			_queries.Merge(_album.Id, album2.Id);

			Assert.AreEqual(1, _album.AllSongs.Count, "Number of songs for source"); // song was not moved
			Assert.AreEqual(3, album2.AllSongs.Count, "Number of songs for target");
			VocaDbAssert.HasSong(album2, _song, 1);
			VocaDbAssert.HasSong(album2, _song2, 3);
			VocaDbAssert.HasSong(album2, _song3, 2);
		}

		/// <summary>
		/// Merge with custom tracks. Custom tracks are added as is.
		/// </summary>
		[TestMethod]
		public void Merge_CustomTracks()
		{
			Save(_album.AddSong(_song, 1, 1));
			Save(_album.AddSong("Bonus song 1", 2, 1));

			var album2 = Save(CreateEntry.Album());
			Save(album2.AddSong(_song, 1, 1));
			Save(album2.AddSong("Bonus song 2", 2, 1));

			_queries.Merge(_album.Id, album2.Id);

			Assert.AreEqual(3, album2.AllSongs.Count, "Number of songs for target");
			VocaDbAssert.HasSong(album2, _song, 1);
			VocaDbAssert.HasSong(album2, "Bonus song 2", 2);
			VocaDbAssert.HasSong(album2, "Bonus song 1", 3);
		}

		[TestMethod]
		public void Revert()
		{
			// Arrange
			_album.Description.English = "Original";
			var oldVer = _repository.HandleTransaction(ctx => _queries.Archive(ctx, _album, AlbumArchiveReason.PropertiesUpdated));
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());
			contract.Description.English = "Updated";
			CallUpdate(contract);

			var entryFromRepo = _repository.Load<Album>(_album.Id);
			Assert.AreEqual("Updated", entryFromRepo.Description.English, "Description was updated");

			// Act
			var result = _queries.RevertToVersion(oldVer.Id);

			// Assert
			Assert.AreEqual(0, result.Warnings.Length, "Number of warnings");

			entryFromRepo = _repository.Load<Album>(result.Id);
			Assert.AreEqual("Original", entryFromRepo.Description.English, "Description was restored");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			Assert.IsNotNull(lastVersion, "Last version is available");
			Assert.AreEqual(AlbumArchiveReason.Reverted, lastVersion.Reason, "Last version archive reason");
			Assert.IsFalse(lastVersion.Diff.Cover.IsChanged, "Picture was not changed");
		}

		/// <summary>
		/// Old version has no image, image will be removed.
		/// </summary>
		[TestMethod]
		public async Task Revert_RemoveImage()
		{
			var oldVer = _repository.HandleTransaction(ctx => _queries.Archive(ctx, _album, AlbumArchiveReason.PropertiesUpdated));
			await CallUpdate(ResourceHelper.TestImage());

			var result = _queries.RevertToVersion(oldVer.Id);

			var entryFromRepo = _repository.Load<Album>(result.Id);
			Assert.IsTrue(PictureData.IsNullOrEmpty(entryFromRepo.CoverPictureData), "Picture data was removed");
			Assert.IsTrue(string.IsNullOrEmpty(entryFromRepo.CoverPictureMime), "Picture MIME was removed");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			Assert.IsNotNull(lastVersion, "Last version is available");
			Assert.AreEqual(2, lastVersion.Version, "Last version number");
			Assert.AreEqual(AlbumArchiveReason.Reverted, lastVersion.Reason, "Last version archive reason");
			Assert.IsTrue(lastVersion.Diff.Cover.IsChanged, "Picture was changed");
		}

		[TestMethod]
		public async Task Update_Names()
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());

			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated album";

			contract = await CallUpdate(contract);
			Assert.AreEqual(_album.Id, contract.Id, "Update album Id as expected");

			var albumFromRepo = _repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", albumFromRepo.DefaultName);
			Assert.AreEqual(1, albumFromRepo.Version, "Version");
			Assert.AreEqual(0, albumFromRepo.AllArtists.Count, "No artists");
			Assert.AreEqual(0, albumFromRepo.AllSongs.Count, "No songs");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(_album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(AlbumEditableFields.Names, archivedVersion.Diff.ChangedFields.Value, "Changed fields");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(_album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");
		}

		[TestMethod]
		public void MoveToTrash()
		{
			_user.AdditionalPermissions.Add(PermissionToken.MoveToTrash);
			_permissionContext.RefreshLoggedUser(_repository);
			var activityEntry = _repository.Save(new AlbumActivityEntry(_album, EntryEditEvent.Updated, _user, null));
			Assert.IsTrue(_repository.Contains(_album), "Album is in repository");
			Assert.IsTrue(_repository.Contains(activityEntry), "Activity entry is in repository");

			var result = _queries.MoveToTrash(_album.Id);

			Assert.IsFalse(_repository.Contains(_album), "Album was deleted from repository");
			Assert.IsFalse(_repository.Contains(activityEntry), "Activity entry was deleted from repository");

			var trashedFromRepo = _repository.Load<TrashedEntry>(result);
			Assert.IsNotNull(trashedFromRepo, "Trashed entry was created");
			Assert.AreEqual(EntryType.Album, trashedFromRepo.EntryType, "Trashed entry type");
			Assert.AreEqual(_album.DefaultName, trashedFromRepo.Name, "Trashed entry name");
		}

		[TestMethod]
		public async Task Update_Tracks()
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());
			var existingSong = CreateEntry.Song(name: "Nebula");
			_repository.Save(existingSong);

			contract.Songs = new[] {
				CreateSongInAlbumEditContract(1, songId: existingSong.Id),
				CreateSongInAlbumEditContract(2, songName: "Anger")
			};

			contract = await CallUpdate(contract);

			var albumFromRepo = _repository.Load(contract.Id);

			Assert.AreEqual(2, albumFromRepo.AllSongs.Count, "Number of songs");

			var track1 = albumFromRepo.GetSongByTrackNum(1, 1);
			Assert.AreEqual(existingSong, track1.Song, "First track");

			var track2 = albumFromRepo.GetSongByTrackNum(1, 2);
			Assert.AreEqual("Anger", track2.Song.DefaultName, "Second track name");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Tracks, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Discs()
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());
			_repository.Save(CreateEntry.AlbumDisc(_album));

			contract.Discs = new[] {
				new AlbumDiscPropertiesContract { Id = 1, Name = "Past" },
				new AlbumDiscPropertiesContract { Id = 2, Name = "Present" }
			};

			contract = await CallUpdate(contract);

			var albumFromRepo = _repository.Load(contract.Id);
			Assert.AreEqual(2, albumFromRepo.Discs.Count, "Number of discs");

			var disc1 = albumFromRepo.GetDisc(1);
			Assert.IsNotNull(disc1, "disc1");
			Assert.AreEqual("Past", disc1.Name, "disc1.Name");

			var disc2 = albumFromRepo.GetDisc(2);
			Assert.IsNotNull(disc2, "disc2");
			Assert.AreEqual("Present", disc2.Name, "disc2.Name");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Discs, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_CoverPicture()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());

			var albumFromRepo = _repository.Load(contract.Id);

			Assert.IsNotNull(albumFromRepo.CoverPictureData, "CoverPictureData");
			Assert.IsNotNull(albumFromRepo.CoverPictureData.Bytes, "Original bytes are saved");
			Assert.AreEqual(MediaTypeNames.Image.Jpeg, albumFromRepo.CoverPictureMime, "CoverPictureData.Mime");

			var thumbData = new EntryThumb(albumFromRepo, albumFromRepo.CoverPictureMime, ImagePurpose.Main);
			Assert.IsFalse(_imagePersister.HasImage(thumbData, ImageSize.Original), "Original file was not created"); // Original saved in CoverPictureData.Bytes
			Assert.IsTrue(_imagePersister.HasImage(thumbData, ImageSize.Thumb), "Thumbnail file was saved");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Cover, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Artists()
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());
			contract.ArtistLinks = new[] {
				CreateArtistForAlbumContract(artistId: _producer.Id),
				CreateArtistForAlbumContract(artistId: _vocalist.Id)
			};

			contract = await CallUpdate(contract);

			var albumFromRepo = _repository.Load(contract.Id);

			Assert.AreEqual(2, albumFromRepo.AllArtists.Count, "Number of artists");

			Assert.IsTrue(albumFromRepo.HasArtist(_producer), "Has producer");
			Assert.IsTrue(albumFromRepo.HasArtist(_vocalist), "Has vocalist");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", albumFromRepo.ArtistString.Default, "Artist string");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Artists, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Artists_CustomArtist()
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());
			contract.ArtistLinks = new[] {
				CreateArtistForAlbumContract(customArtistName: "Custom artist", roles: ArtistRoles.Composer)
			};

			contract = await CallUpdate(contract);

			var albumFromRepo = _repository.Load(contract.Id);

			Assert.AreEqual(1, albumFromRepo.AllArtists.Count, "Number of artists");
			Assert.IsTrue(albumFromRepo.AllArtists.Any(a => a.Name == "Custom artist"), "Has custom artist");
			Assert.AreEqual("Custom artist", albumFromRepo.ArtistString.Default, "Artist string");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Artists, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Artists_Notify()
		{
			Save(_user2.AddArtist(_vocalist));

			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.Default, new InMemoryImagePersister());
			contract.ArtistLinks = contract.ArtistLinks.Concat(new[] { CreateArtistForAlbumContract(_vocalist.Id) }).ToArray();

			await _queries.UpdateBasicProperties(contract, null);

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(_user2, notification.Receiver, "Receiver");
		}

		[TestMethod]
		public void AddReview()
		{
			AlbumReviewContract AddReview(int id, string text, string title, string languageCode) => _queries.AddReview(_album.Id, new AlbumReviewContract
			{
				Id = id,
				AlbumId = _album.Id,
				Text = text,
				User = new UserForApiContract(_user, null, UserOptionalFields.None),
				Title = title,
				LanguageCode = languageCode,
			});

			void AssertReview(int id, string message, string title, string languageCode, bool deleted, AlbumReview review)
			{
				Assert.AreEqual(id, review.Id);
				Assert.AreEqual(message, review.Message);
				Assert.AreEqual(_user, review.Author, "Author");
				Assert.AreEqual(_album, review.Entry, "Album");
				Assert.AreEqual(title, review.Title);
				Assert.AreEqual(languageCode, review.LanguageCode);
				Assert.AreEqual(deleted, review.Deleted);
			}

			// 1. User posts review (review is created)
			var review = AddReview(
				id: 0,
				text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				title: "Title",
				languageCode: "ja");

			var reviewFromRepo = _repository.Load<AlbumReview>(review.Id);
			AssertReview(
				review.Id,
				message: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				title: "Title",
				languageCode: "ja",
				deleted: false,
				reviewFromRepo);

			// 2. User deletes review (review is marked deleted)
			_queries.DeleteReview(reviewFromRepo.Id);

			reviewFromRepo = _repository.Load<AlbumReview>(review.Id);
			AssertReview(
				review.Id,
				message: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				title: "Title",
				languageCode: "ja",
				deleted: true,
				reviewFromRepo);

			// 3. User posts new review (review is created)
			var review2 = AddReview(
				id: 0,
				text: "いろはにほへと ちりぬるを わかよたれそ つねならむ うゐのおくやま けふこえて あさきゆめみし ゑひもせす",
				title: "いろはうた",
				languageCode: "ja");

			var review2FromRepo = _repository.Load<AlbumReview>(review2.Id);
			AssertReview(
				review2.Id,
				message: "いろはにほへと ちりぬるを わかよたれそ つねならむ うゐのおくやま けふこえて あさきゆめみし ゑひもせす",
				title: "いろはうた",
				languageCode: "ja",
				deleted: false,
				review2FromRepo);

			// 4. User updates review
			var result = AddReview(
				id: review2FromRepo.Id,
				text: "色は匂へど 散りぬるを 我が世誰ぞ 常ならむ 有為の奥山 今日越えて 浅き夢見じ 酔ひもせず",
				title: "いろは歌",
				languageCode: review2FromRepo.LanguageCode);

			var resultFromRepo = _repository.Load<AlbumReview>(result.Id);
			AssertReview(
				review2FromRepo.Id,
				message: "色は匂へど 散りぬるを 我が世誰ぞ 常ならむ 有為の奥山 今日越えて 浅き夢見じ 酔ひもせず",
				title: "いろは歌",
				languageCode: "ja",
				deleted: false,
				resultFromRepo);

			reviewFromRepo = _repository.Load<AlbumReview>(review.Id);
			AssertReview(
				review.Id,
				message: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				title: "Title",
				languageCode: "ja",
				deleted: true,
				reviewFromRepo);
		}
	}
}
