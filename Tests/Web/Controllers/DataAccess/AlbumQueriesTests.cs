using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="AlbumQueries"/>.
	/// </summary>
	[TestClass]
	public class AlbumQueriesTests {

		private Album album;
		private InMemoryImagePersister imagePersister;
		private FakeUserMessageMailer mailer;
		private CreateAlbumContract newAlbumContract;
		private FakePermissionContext permissionContext;
		private Artist producer;
		private FakeAlbumRepository repository;
		private AlbumQueries queries;
		private Song song;
		private Song song2;
		private Song song3;
		private User user;
		private User user2;
		private Artist vocalist;
		private Artist vocalist2;

		private void AssertHasArtist(Album album, string artistName, ArtistRoles? roles) {
			VocaDbAssert.HasArtist(album, artistName, roles);
		}

		private void AssertHasArtist(Album album, Artist artist, ArtistRoles? roles) {
			VocaDbAssert.HasArtist(album, artist, roles);
		}

		private SongInAlbumEditContract CreateSongInAlbumEditContract(int trackNumber, int songId = 0, string songName = null) {
			return new SongInAlbumEditContract { DiscNumber = 1, TrackNumber = trackNumber, SongId = songId, SongName = songName, Artists = new ArtistContract[0] };
		}

		private ArtistForAlbumContract CreateArtistForAlbumContract(int artistId = 0, string customArtistName = null, ArtistRoles roles = ArtistRoles.Default) {

			if (artistId != 0)
				return new ArtistForAlbumContract { Artist = new ArtistContract { Id = artistId }, Roles = roles };
			else
				return new ArtistForAlbumContract { Name = customArtistName, Roles = roles };

		}

		private AlbumForEditContract CallUpdate(AlbumForEditContract contract) {
			return queries.UpdateBasicProperties(contract, null);
		}

		private AlbumForEditContract CallUpdate(Stream image) {
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			using (var stream = image) {
				return queries.UpdateBasicProperties(contract, new EntryPictureFileContract { UploadedFile = stream, Mime = MediaTypeNames.Image.Jpeg });
			}		
		}

		private void Save<T>(params T[] entity) {
			repository.Save(entity);
		}

		private T Save<T>(T entity) {
			return repository.Save(entity);
		}

		[TestInitialize]
		public void SetUp() {
			
			producer = CreateEntry.Producer();
			vocalist = CreateEntry.Vocalist(name: "Hatsune Miku");
			vocalist2 = CreateEntry.Vocalist(name: "Rin");

			album = CreateEntry.Album(id: 39, name: "Synthesis");
			repository = new FakeAlbumRepository(album);
			foreach (var name in album.Names)
				Save(name);
			user = CreateEntry.User(1, "Miku");
			user.GroupId = UserGroupId.Trusted;
			user2 = CreateEntry.User(2, "Luka");
			Save(user, user2);
			Save(producer, vocalist, vocalist2);

			song = Save(CreateEntry.Song(name: "Nebula"));
			song2 = Save(CreateEntry.Song(name: "Anger"));
			song3 = Save(CreateEntry.Song(name: "Resistance"));

			permissionContext = new FakePermissionContext(user);
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			newAlbumContract = new CreateAlbumContract {
				DiscType = DiscType.Album,
				Names = new[] {
					new LocalizedStringContract("Another Dimensions", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistContract(producer, ContentLanguagePreference.Default),
					new ArtistContract(vocalist, ContentLanguagePreference.Default), 
				}
			};

			imagePersister = new InMemoryImagePersister();
			mailer = new FakeUserMessageMailer();
			queries = new AlbumQueries(repository, permissionContext, entryLinkFactory, imagePersister, imagePersister, mailer);

		}

		[TestMethod]
		public void Create() {

			var result = queries.Create(newAlbumContract);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Another Dimensions", result.Name, "Name");

			album = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Another Dimensions"));

			Assert.IsNotNull(album, "Album was saved to repository");
			Assert.AreEqual("Another Dimensions", album.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, album.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, album.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(album.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", album.ArtistString.Default, "ArtistString");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			queries.Create(newAlbumContract);

		}

		[TestMethod]
		public void CreateComment() {

			var result = queries.CreateComment(39, "Hello world");
			Assert.IsNotNull(result, "Result");

			var comment = repository.List<AlbumComment>().FirstOrDefault();
			Assert.IsNotNull(comment, "Comment was saved");
			Assert.AreEqual(user, comment.Author, "Author");
			Assert.AreEqual(album, comment.Album, "Album");
			Assert.AreEqual("Hello world", comment.Message, "Comment message");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void CreateComment_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);
			
			queries.CreateComment(39, "Hello world");

		}

		[TestMethod]
		public void GetCoverPictureThumb() {
			
			var contract = CallUpdate(ResourceHelper.TestImage());

			var result = queries.GetCoverPictureThumb(contract.Id);

			Assert.IsNotNull(result, "result");
			Assert.IsNotNull(result.Picture, "Picture");
			Assert.IsNotNull(result.Picture.Bytes, "Picture content");
			Assert.AreEqual(contract.CoverPictureMime, result.Picture.Mime, "Picture MIME");
			Assert.AreEqual(contract.Id, result.EntryId, "EntryId");

		}

		[TestMethod]
		public void Merge_ToEmpty() {
			
			Save(album.AddArtist(producer));
			Save(album.AddArtist(vocalist));
			Save(album.AddSong(song, 1, 1));

			var album2 = CreateEntry.Album();
			Save(album2);

			queries.Merge(album.Id, album2.Id);

			Assert.IsTrue(album.Deleted, "Original was deleted");
			Assert.AreEqual(0, album.AllArtists.Count, "All artists removed from original");
			Assert.AreEqual(0, album.AllSongs.Count, "All songs removed from original");

			Assert.AreEqual("Synthesis", album2.DefaultName, "Name");
			Assert.AreEqual(2, album2.AllArtists.Count, "Number of artists");
			Assert.AreEqual(1, album2.AllSongs.Count, "Number of songs");
			Assert.AreEqual("Nebula", album2.AllSongs.First().Song.DefaultName);

			var mergeRecord = repository.List<AlbumMergeRecord>().FirstOrDefault(m => m.Source == album.Id);
			Assert.IsNotNull(mergeRecord, "Merge record was created");
			Assert.AreEqual(album2.Id, mergeRecord.Target.Id, "mergeRecord.Target.Id");

		}

		[TestMethod]
		public void Merge_WithArtists() {

			Save(album.AddArtist(producer, false, ArtistRoles.Instrumentalist));
			Save(album.AddArtist(vocalist, false, ArtistRoles.Default));

			var album2 = Save(CreateEntry.Album());
			Save(album2.AddArtist(vocalist, false, ArtistRoles.Mastering));
			Save(album2.AddArtist("Kaito", true, ArtistRoles.Default));

			queries.Merge(album.Id, album2.Id);

			Assert.AreEqual(1, album.AllArtists.Count, "Number of artists for source"); // Vocalist was not moved
			Assert.AreEqual(3, album2.AllArtists.Count, "Number of artists for target");
			AssertHasArtist(album2, producer, ArtistRoles.Instrumentalist);
			AssertHasArtist(album2, vocalist, ArtistRoles.Mastering);
			AssertHasArtist(album2, "Kaito", ArtistRoles.Default);

		}

		[TestMethod]
		public void Merge_WithTracks() {

			Save(album.AddSong(song, 1, 1));
			Save(album.AddSong(song2, 2, 1));

			var album2 = Save(CreateEntry.Album());
			Save(album2.AddSong(song, 1, 1));
			Save(album2.AddSong(song3, 2, 1));

			queries.Merge(album.Id, album2.Id);

			Assert.AreEqual(1, album.AllSongs.Count, "Number of songs for source"); // song was not moved
			Assert.AreEqual(3, album2.AllSongs.Count, "Number of songs for target");
			VocaDbAssert.HasSong(album2, song, 1);
			VocaDbAssert.HasSong(album2, song2, 3);
			VocaDbAssert.HasSong(album2, song3, 2);

		}

		/// <summary>
		/// Merge with custom tracks. Custom tracks are added as is.
		/// </summary>
		[TestMethod]
		public void Merge_CustomTracks() {
			
			Save(album.AddSong(song, 1, 1));
			Save(album.AddSong("Bonus song 1", 2, 1));

			var album2 = Save(CreateEntry.Album());
			Save(album2.AddSong(song, 1, 1));
			Save(album2.AddSong("Bonus song 2", 2, 1));

			queries.Merge(album.Id, album2.Id);

			Assert.AreEqual(3, album2.AllSongs.Count, "Number of songs for target");
			VocaDbAssert.HasSong(album2, song, 1);
			VocaDbAssert.HasSong(album2, "Bonus song 2", 2);
			VocaDbAssert.HasSong(album2, "Bonus song 1", 3);

		}

		[TestMethod]
		public void Update_Names() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);

			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated album";

			contract = CallUpdate(contract);
			Assert.AreEqual(album.Id, contract.Id, "Update album Id as expected");

			var albumFromRepo = repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", albumFromRepo.DefaultName);
			Assert.AreEqual(1, albumFromRepo.Version, "Version");
			Assert.AreEqual(0, albumFromRepo.AllArtists.Count, "No artists");
			Assert.AreEqual(0, albumFromRepo.AllSongs.Count, "No songs");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(album, archivedVersion.Album, "Archived version album");
			Assert.AreEqual(AlbumArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(AlbumEditableFields.Names, archivedVersion.Diff.ChangedFields, "Changed fields");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(album, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		public void Update_Tracks() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			var existingSong = CreateEntry.Song(name: "Nebula");
			repository.Save(existingSong);

			contract.Songs = new[] {
				CreateSongInAlbumEditContract(1, songId: existingSong.Id),
				CreateSongInAlbumEditContract(2, songName: "Anger")
			};

			contract = CallUpdate(contract);

			var albumFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(2, albumFromRepo.AllSongs.Count, "Number of songs");

			var track1 = albumFromRepo.GetSongByTrackNum(1, 1);
			Assert.AreEqual(existingSong, track1.Song, "First track");

			var track2 = albumFromRepo.GetSongByTrackNum(1, 2);
			Assert.AreEqual("Anger", track2.Song.DefaultName, "Second track name");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Tracks, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_CoverPicture() {
			
			var contract = CallUpdate(ResourceHelper.TestImage());

			var albumFromRepo = repository.Load(contract.Id);

			Assert.IsNotNull(albumFromRepo.CoverPictureData, "CoverPictureData");
			Assert.IsNotNull(albumFromRepo.CoverPictureData.Bytes, "Original bytes are saved");
			Assert.IsNull(albumFromRepo.CoverPictureData.Thumb250, "Thumb bytes not saved anymore");
			Assert.AreEqual(MediaTypeNames.Image.Jpeg, albumFromRepo.CoverPictureMime, "CoverPictureData.Mime");

			var thumbData = new EntryThumb(albumFromRepo, albumFromRepo.CoverPictureMime);
			Assert.IsFalse(imagePersister.HasImage(thumbData, ImageSize.Original), "Original file was not created"); // Original saved in CoverPictureData.Bytes
			Assert.IsTrue(imagePersister.HasImage(thumbData, ImageSize.Thumb), "Thumbnail file was saved");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Cover, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_Artists() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			contract.ArtistLinks = new [] {
				CreateArtistForAlbumContract(artistId: producer.Id), 
				CreateArtistForAlbumContract(artistId: vocalist.Id)
			};

			contract = CallUpdate(contract);

			var albumFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(2, albumFromRepo.AllArtists.Count, "Number of artists");

			Assert.IsTrue(albumFromRepo.HasArtist(producer), "Has producer");
			Assert.IsTrue(albumFromRepo.HasArtist(vocalist), "Has vocalist");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", albumFromRepo.ArtistString.Default, "Artist string");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Artists, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_Artists_CustomArtist() {
			
			var contract = new AlbumForEditContract(album, ContentLanguagePreference.English);
			contract.ArtistLinks = new [] {
				CreateArtistForAlbumContract(customArtistName: "Custom artist", roles: ArtistRoles.Composer)
			};

			contract = CallUpdate(contract);

			var albumFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(1, albumFromRepo.AllArtists.Count, "Number of artists");
			Assert.IsTrue(albumFromRepo.AllArtists.Any(a => a.Name == "Custom artist"), "Has custom artist");
			Assert.AreEqual("Custom artist", albumFromRepo.ArtistString.Default, "Artist string");

			var archivedVersion = repository.List<ArchivedAlbumVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(AlbumEditableFields.Artists, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

		[TestMethod]
		public void Update_Artists_Notify() {
			
			Save(user2.AddArtist(vocalist));

			var contract = new AlbumForEditContract(album, ContentLanguagePreference.Default);
			contract.ArtistLinks = contract.ArtistLinks.Concat(new [] { CreateArtistForAlbumContract(vocalist.Id)}).ToArray();

			queries.UpdateBasicProperties(contract, null);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.Receiver, "Receiver");

		}

	}

}
