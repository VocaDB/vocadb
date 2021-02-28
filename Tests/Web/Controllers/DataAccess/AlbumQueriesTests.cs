#nullable disable

using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
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

			result.Should().NotBeNull("result");
			result.Name.Should().Be("Another Dimensions", "Name");

			_album = _repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Another Dimensions"));

			_album.Should().NotBeNull("Album was saved to repository");
			_album.DefaultName.Should().Be("Another Dimensions", "Name");
			_album.Names.SortNames.DefaultLanguage.Should().Be(ContentLanguageSelection.English, "Default language should be English");
			_album.AllArtists.Count.Should().Be(2, "Artists count");
			VocaDbAssert.ContainsArtists(_album.AllArtists, "Tripshots", "Hatsune Miku");
			_album.ArtistString.Default.Should().Be("Tripshots feat. Hatsune Miku", "ArtistString");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Album.Should().Be(_album, "Archived version album");
			archivedVersion.Reason.Should().Be(AlbumArchiveReason.Created, "Archived version reason");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			activityEntry.Should().NotBeNull("Activity entry was created");
			activityEntry.EntryBase.Should().Be(_album, "Activity entry's entry");
			activityEntry.EditEvent.Should().Be(EntryEditEvent.Created, "Activity entry event type");
		}

		[TestMethod]
		public void Create_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			_queries.Awaiting(subject => subject.Create(_newAlbumContract)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void CreateComment()
		{
			var result = CreateComment(39, "Hello world");
			result.Should().NotBeNull("Result");

			var comment = _repository.List<Comment>().FirstOrDefault();
			comment.Should().NotBeNull("Comment was saved");
			comment.Author.Should().Be(_user, "Author");
			comment.Entry.Should().Be(_album, "Album");
			comment.Message.Should().Be("Hello world", "Comment message");
		}

		[TestMethod]
		public void CreateComment_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			this.Invoking(subject => subject.CreateComment(39, "Hello world")).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public async Task GetCoverPictureThumb()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());

			var result = _queries.GetCoverPictureThumb(contract.Id);

			result.Should().NotBeNull("result");
			result.Picture.Should().NotBeNull("Picture");
			result.Picture.Bytes.Should().NotBeNull("Picture content");
			result.Picture.Mime.Should().Be(contract.CoverPictureMime, "Picture MIME");
			result.EntryId.Should().Be(contract.Id, "EntryId");
		}

		[TestMethod]
		public void GetAlbumDetails()
		{
			_repository.Save(_album.AddSong(_song, 1, 1));
			_repository.Save(_album.AddSong(_song2, 2, 1));
			_repository.Save(_user.AddSongToFavorites(_song, SongVoteRating.Favorite));

			var result = _queries.GetAlbumDetails(_album.Id, "miku@vocaloid.eu");

			result.Songs.Length.Should().Be(2, "Number of songs");
			var track = result.Songs[0];
			track.Rating.Should().Be(SongVoteRating.Favorite, "First track rating");
			result.Songs[1].Rating.Should().Be(SongVoteRating.Nothing, "Second track rating");
		}

		[TestMethod]
		public void GetAlbumDetails_CustomTrack()
		{
			_repository.Save(_album.AddSong("Miku Miku", 1, 1));
			_repository.Save(_user.AddSongToFavorites(_song, SongVoteRating.Favorite));

			var result = _queries.GetAlbumDetails(_album.Id, "miku@vocaloid.eu");

			result.Songs.Length.Should().Be(1, "Number of songs");
		}

		[TestMethod]
		public void GetAlbumDetails_NotLoggedIn()
		{
			_repository.Save(_album.AddSong("Miku Miku", 1, 1));
			_permissionContext.LogOff();

			var result = _queries.GetAlbumDetails(_album.Id, "miku@vocaloid.eu");
			result.Should().NotBeNull("result");
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

			result.Length.Should().Be(2, "Number of tag suggestions");
			result.Any(r => r.Tag.Name == "vocarock").Should().BeTrue("First tag was returned");
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

			_album.Deleted.Should().BeTrue("Original was deleted");
			_album.AllArtists.Count.Should().Be(0, "All artists removed from original");
			_album.AllSongs.Count.Should().Be(0, "All songs removed from original");

			album2.DefaultName.Should().Be("Synthesis", "Name");
			album2.AllArtists.Count.Should().Be(2, "Number of artists");
			album2.AllSongs.Count.Should().Be(1, "Number of songs");
			album2.AllSongs.First().Song.DefaultName.Should().Be("Nebula");

			var mergeRecord = _repository.List<AlbumMergeRecord>().FirstOrDefault(m => m.Source == _album.Id);
			mergeRecord.Should().NotBeNull("Merge record was created");
			mergeRecord.Target.Id.Should().Be(album2.Id, "mergeRecord.Target.Id");
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

			_album.AllArtists.Count.Should().Be(1, "Number of artists for source"); // Vocalist was not moved
			album2.AllArtists.Count.Should().Be(3, "Number of artists for target");
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

			_album.AllSongs.Count.Should().Be(1, "Number of songs for source"); // song was not moved
			album2.AllSongs.Count.Should().Be(3, "Number of songs for target");
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

			album2.AllSongs.Count.Should().Be(3, "Number of songs for target");
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
			entryFromRepo.Description.English.Should().Be("Updated", "Description was updated");

			// Act
			var result = _queries.RevertToVersion(oldVer.Id);

			// Assert
			result.Warnings.Length.Should().Be(0, "Number of warnings");

			entryFromRepo = _repository.Load<Album>(result.Id);
			entryFromRepo.Description.English.Should().Be("Original", "Description was restored");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			lastVersion.Should().NotBeNull("Last version is available");
			lastVersion.Reason.Should().Be(AlbumArchiveReason.Reverted, "Last version archive reason");
			lastVersion.Diff.Cover.IsChanged.Should().BeFalse("Picture was not changed");
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
			PictureData.IsNullOrEmpty(entryFromRepo.CoverPictureData).Should().BeTrue("Picture data was removed");
			string.IsNullOrEmpty(entryFromRepo.CoverPictureMime).Should().BeTrue("Picture MIME was removed");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			lastVersion.Should().NotBeNull("Last version is available");
			lastVersion.Version.Should().Be(2, "Last version number");
			lastVersion.Reason.Should().Be(AlbumArchiveReason.Reverted, "Last version archive reason");
			lastVersion.Diff.Cover.IsChanged.Should().BeTrue("Picture was changed");
		}

		[TestMethod]
		public async Task Update_Names()
		{
			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.English, new InMemoryImagePersister());

			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated album";

			contract = await CallUpdate(contract);
			contract.Id.Should().Be(_album.Id, "Update album Id as expected");

			var albumFromRepo = _repository.Load(contract.Id);
			albumFromRepo.DefaultName.Should().Be("Replaced name");
			albumFromRepo.Version.Should().Be(1, "Version");
			albumFromRepo.AllArtists.Count.Should().Be(0, "No artists");
			albumFromRepo.AllSongs.Count.Should().Be(0, "No songs");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Album.Should().Be(_album, "Archived version album");
			archivedVersion.Reason.Should().Be(AlbumArchiveReason.PropertiesUpdated, "Archived version reason");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(AlbumEditableFields.Names, "Changed fields");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			activityEntry.Should().NotBeNull("Activity entry was created");
			activityEntry.EntryBase.Should().Be(_album, "Activity entry's entry");
			activityEntry.EditEvent.Should().Be(EntryEditEvent.Updated, "Activity entry event type");
		}

		[TestMethod]
		public void MoveToTrash()
		{
			_user.AdditionalPermissions.Add(PermissionToken.MoveToTrash);
			_permissionContext.RefreshLoggedUser(_repository);
			var activityEntry = _repository.Save(new AlbumActivityEntry(_album, EntryEditEvent.Updated, _user, null));
			_repository.Contains(_album).Should().BeTrue("Album is in repository");
			_repository.Contains(activityEntry).Should().BeTrue("Activity entry is in repository");

			var result = _queries.MoveToTrash(_album.Id);

			_repository.Contains(_album).Should().BeFalse("Album was deleted from repository");
			_repository.Contains(activityEntry).Should().BeFalse("Activity entry was deleted from repository");

			var trashedFromRepo = _repository.Load<TrashedEntry>(result);
			trashedFromRepo.Should().NotBeNull("Trashed entry was created");
			trashedFromRepo.EntryType.Should().Be(EntryType.Album, "Trashed entry type");
			trashedFromRepo.Name.Should().Be(_album.DefaultName, "Trashed entry name");
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

			albumFromRepo.AllSongs.Count.Should().Be(2, "Number of songs");

			var track1 = albumFromRepo.GetSongByTrackNum(1, 1);
			track1.Song.Should().Be(existingSong, "First track");

			var track2 = albumFromRepo.GetSongByTrackNum(1, 2);
			track2.Song.DefaultName.Should().Be("Anger", "Second track name");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(AlbumEditableFields.Tracks, "Changed fields");
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
			albumFromRepo.Discs.Count.Should().Be(2, "Number of discs");

			var disc1 = albumFromRepo.GetDisc(1);
			disc1.Should().NotBeNull("disc1");
			disc1.Name.Should().Be("Past", "disc1.Name");

			var disc2 = albumFromRepo.GetDisc(2);
			disc2.Should().NotBeNull("disc2");
			disc2.Name.Should().Be("Present", "disc2.Name");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(AlbumEditableFields.Discs, "Changed fields");
		}

		[TestMethod]
		public async Task Update_CoverPicture()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());

			var albumFromRepo = _repository.Load(contract.Id);

			albumFromRepo.CoverPictureData.Should().NotBeNull("CoverPictureData");
			albumFromRepo.CoverPictureData.Bytes.Should().NotBeNull("Original bytes are saved");
			albumFromRepo.CoverPictureMime.Should().Be(MediaTypeNames.Image.Jpeg, "CoverPictureData.Mime");

			var thumbData = new EntryThumb(albumFromRepo, albumFromRepo.CoverPictureMime, ImagePurpose.Main);
			_imagePersister.HasImage(thumbData, ImageSize.Original).Should().BeFalse("Original file was not created"); // Original saved in CoverPictureData.Bytes
			_imagePersister.HasImage(thumbData, ImageSize.Thumb).Should().BeTrue("Thumbnail file was saved");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(AlbumEditableFields.Cover, "Changed fields");
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

			albumFromRepo.AllArtists.Count.Should().Be(2, "Number of artists");

			albumFromRepo.HasArtist(_producer).Should().BeTrue("Has producer");
			albumFromRepo.HasArtist(_vocalist).Should().BeTrue("Has vocalist");
			albumFromRepo.ArtistString.Default.Should().Be("Tripshots feat. Hatsune Miku", "Artist string");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(AlbumEditableFields.Artists, "Changed fields");
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

			albumFromRepo.AllArtists.Count.Should().Be(1, "Number of artists");
			albumFromRepo.AllArtists.Any(a => a.Name == "Custom artist").Should().BeTrue("Has custom artist");
			albumFromRepo.ArtistString.Default.Should().Be("Custom artist", "Artist string");

			var archivedVersion = _repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(AlbumEditableFields.Artists, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Artists_Notify()
		{
			Save(_user2.AddArtist(_vocalist));

			var contract = new AlbumForEditContract(_album, ContentLanguagePreference.Default, new InMemoryImagePersister());
			contract.ArtistLinks = contract.ArtistLinks.Concat(new[] { CreateArtistForAlbumContract(_vocalist.Id) }).ToArray();

			await _queries.UpdateBasicProperties(contract, null);

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			notification.Should().NotBeNull("Notification was created");
			notification.Receiver.Should().Be(_user2, "Receiver");
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
				review.Id.Should().Be(id);
				review.Message.Should().Be(message);
				review.Author.Should().Be(_user, "Author");
				review.Entry.Should().Be(_album, "Album");
				review.Title.Should().Be(title);
				review.LanguageCode.Should().Be(languageCode);
				review.Deleted.Should().Be(deleted);
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
