#nullable disable

using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Caching;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources.Messages;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="ArtistQueries"/>.
	/// </summary>
	[TestClass]
	public class ArtistQueriesTests
	{
		private Artist _artist;
		private CreateArtistContract _newArtistContract;
		private InMemoryImagePersister _imagePersister;
		private FakePermissionContext _permissionContext;
		private ArtistQueries _queries;
		private FakeArtistRepository _repository;
		/// <summary>
		/// Logged in user
		/// </summary>
		private User _user;
		private User _user2;
		private Artist _vocalist;

		private async Task<ArtistForEditContract> CallUpdate(ArtistForEditContract contract)
		{
			contract.Id = await _queries.Update(contract, null, _permissionContext);
			return contract;
		}

		private async Task<ArtistForEditContract> CallUpdate(Stream image, string mime = MediaTypeNames.Image.Jpeg)
		{
			var contract = new ArtistForEditContract(_artist, ContentLanguagePreference.English, new InMemoryImagePersister());
			using (var stream = image)
			{
				contract.Id = await _queries.Update(contract, new EntryPictureFileContract(stream, mime, (int)stream.Length, ImagePurpose.Main), _permissionContext);
			}
			return contract;
		}

		[TestInitialize]
		public void SetUp()
		{
			_artist = CreateEntry.Producer(name: "Tripshots");
			_vocalist = CreateEntry.Vocalist(name: "Hatsune Miku");
			_repository = new FakeArtistRepository(_artist, _vocalist);
			var weblink = new ArtistWebLink(_artist, "Website", "http://tripshots.net", WebLinkCategory.Official, disabled: false);
			_artist.WebLinks.Add(weblink);
			_repository.Save(weblink);
			_repository.SaveNames(_artist, _vocalist);

			_user = CreateEntry.User(name: "Miku", group: UserGroupId.Moderator);
			_user2 = CreateEntry.User(name: "Rin", group: UserGroupId.Regular);
			_repository.Save(_user);
			_permissionContext = new FakePermissionContext(_user);
			_imagePersister = new InMemoryImagePersister();

			_queries = new ArtistQueries(_repository, _permissionContext, new FakeEntryLinkFactory(), _imagePersister, _imagePersister, MemoryCache.Default,
				new FakeUserIconFactory(), new EnumTranslations(), _imagePersister);

			_newArtistContract = new CreateArtistContract
			{
				ArtistType = ArtistType.Producer,
				Description = string.Empty,
				Names = new[] {
					new LocalizedStringContract("Tripshots", ContentLanguageSelection.English)
				},
				WebLink = new WebLinkContract("http://tripshots.net/", "Website", WebLinkCategory.Official, disabled: false)
			};
		}

		private (bool created, ArtistReport report) CallCreateReport(ArtistReportType reportType, int? versionNumber = null, Artist artist = null)
		{
			artist ??= _artist;
			var result = _queries.CreateReport(artist.Id, reportType, "39.39.39.39", "It's Miku, not Rin", versionNumber);
			var report = _repository.Load<ArtistReport>(result.reportId);
			return (result.created, report);
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await _queries.Create(_newArtistContract);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Tripshots", result.Name, "Name");

			_artist = _repository.Load(result.Id);

			Assert.IsNotNull(_artist, "Artist was saved to repository");
			Assert.AreEqual("Tripshots", _artist.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, _artist.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(1, _artist.WebLinks.Count, "Weblinks count");

			var archivedVersion = _repository.List<ArchivedArtistVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(_artist, archivedVersion.Artist, "Archived version artist");
			Assert.AreEqual(ArtistArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(_artist, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public async Task Create_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			await _queries.Create(_newArtistContract);
		}

		[TestMethod]
		public void CreateReport()
		{
			var editor = _user2;
			_repository.Save(ArchivedArtistVersion.Create(_artist, new ArtistDiff(), new AgentLoginData(editor), ArtistArchiveReason.PropertiesUpdated, string.Empty));
			var (created, report) = CallCreateReport(ArtistReportType.InvalidInfo);

			created.Should().BeTrue("Report was created");
			report.EntryBase.Id.Should().Be(_artist.Id);
			report.User.Should().Be(_user);
			report.ReportType.Should().Be(ArtistReportType.InvalidInfo);

			var notification = _repository.List<UserMessage>().FirstOrDefault();
			notification.Should().NotBeNull("notification was created");
			notification.Receiver.Should().Be(editor, "notification receiver is editor");
			notification.Subject.Should().Be(string.Format(EntryReportStrings.EntryVersionReportTitle, _artist.DefaultName));
		}

		[TestMethod]
		public void CreateReport_OwnershipClaim()
		{
			var editor = _user2;
			_repository.Save(ArchivedArtistVersion.Create(_artist, new ArtistDiff(), new AgentLoginData(editor), ArtistArchiveReason.PropertiesUpdated, string.Empty));
			var (created, _) = CallCreateReport(ArtistReportType.OwnershipClaim);

			created.Should().BeTrue("Report was created");

			_repository.List<UserMessage>().Should().BeEmpty("No notification created");
		}

		[TestMethod]
		public void FindDuplicates_Name()
		{
			var result = _queries.FindDuplicates(new[] { _artist.DefaultName }, string.Empty);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Length, "Number of results");
			Assert.AreEqual(_artist.Id, result[0].Id, "Matched artist");
		}

		[TestMethod]
		public void FindDuplicates_Link()
		{
			var result = _queries.FindDuplicates(new string[0], "http://tripshots.net");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Length, "Number of results");
			Assert.AreEqual(_artist.Id, result[0].Id, "Matched artist");
		}

		[TestMethod]
		public void FindDuplicates_DifferentScheme()
		{
			var result = _queries.FindDuplicates(new string[0], "https://tripshots.net");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Length, "Number of results");
			Assert.AreEqual(_artist.Id, result[0].Id, "Matched artist");
		}

		[TestMethod]
		public void FindDuplicates_IgnoreNullsAndEmpty()
		{
			var result = _queries.FindDuplicates(new[] { null, string.Empty }, string.Empty);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(0, result.Length, "Number of results");
		}

		[TestMethod]
		public void FindDuplicates_Link_IgnoreDeleted()
		{
			_artist.Deleted = true;
			var result = _queries.FindDuplicates(new string[0], "http://tripshots.net");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(0, result.Length, "Number of results");
		}

		[TestMethod]
		public void FindDuplicates_Link_IgnoreInvalidLink()
		{
			var result = _queries.FindDuplicates(new string[0], "Miku!");
			Assert.AreEqual(0, result?.Length, "Number of results");
		}

		[TestMethod]
		public async Task GetCoverPictureThumb()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());
			contract.PictureMime = "image/jpeg";

			var result = _queries.GetPictureThumb(contract.Id);

			Assert.IsNotNull(result, "result");
			Assert.IsNotNull(result.Picture, "Picture");
			Assert.IsNotNull(result.Picture.Bytes, "Picture content");
			Assert.AreEqual(contract.PictureMime, result.Picture.Mime, "Picture MIME");
			Assert.AreEqual(contract.Id, result.EntryId, "EntryId");
		}

		[TestMethod]
		public void GetDetails()
		{
			var result = _queries.GetDetails(_artist.Id, "39.39.39.39");

			Assert.AreEqual("Tripshots", result.Name, "Name");

			var hit = _repository.List<ArtistHit>().FirstOrDefault(a => a.Entry.Equals(_artist));
			Assert.IsNotNull(hit, "Hit was created");
			Assert.AreEqual(_user.Id, hit.Agent, "Hit creator");
		}

		[TestMethod]
		public void GetTagSuggestions()
		{
			var song = _repository.Save(CreateEntry.Song());
		}

		[TestMethod]
		public async Task Revert()
		{
			// Arrange
			_artist.Description.English = "Original";
			var oldVer = await _repository.HandleTransactionAsync(ctx => _queries.ArchiveAsync(ctx, _artist, ArtistArchiveReason.PropertiesUpdated));
			var contract = new ArtistForEditContract(_artist, ContentLanguagePreference.English, new InMemoryImagePersister());
			contract.Description.English = "Updated";
			await CallUpdate(contract);

			var entryFromRepo = _repository.Load<Artist>(_artist.Id);
			Assert.AreEqual("Updated", entryFromRepo.Description.English, "Description was updated");

			// Act
			var result = await _queries.RevertToVersion(oldVer.Id);

			// Assert
			Assert.AreEqual(0, result.Warnings.Length, "Number of warnings");

			entryFromRepo = _repository.Load<Artist>(result.Id);
			Assert.AreEqual("Original", entryFromRepo.Description.English, "Description was restored");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			Assert.IsNotNull(lastVersion, "Last version is available");
			Assert.AreEqual(ArtistArchiveReason.Reverted, lastVersion.Reason, "Last version archive reason");
			Assert.IsFalse(lastVersion.Diff.Picture.IsChanged, "Picture was not changed");
		}

		/// <summary>
		/// Old version has no image, image will be removed.
		/// </summary>
		[TestMethod]
		public async Task Revert_RemoveImage()
		{
			var oldVer = await _repository.HandleTransactionAsync(ctx => _queries.ArchiveAsync(ctx, _artist, ArtistArchiveReason.PropertiesUpdated));
			await CallUpdate(ResourceHelper.TestImage());

			var result = await _queries.RevertToVersion(oldVer.Id);

			var entryFromRepo = _repository.Load<Artist>(result.Id);
			Assert.IsTrue(PictureData.IsNullOrEmpty(entryFromRepo.Picture), "Picture data was removed");
			Assert.IsTrue(string.IsNullOrEmpty(entryFromRepo.PictureMime), "Picture MIME was removed");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			Assert.IsNotNull(lastVersion, "Last version is available");
			Assert.AreEqual(2, lastVersion.Version, "Last version number");
			Assert.AreEqual(ArtistArchiveReason.Reverted, lastVersion.Reason, "Last version archive reason");
			Assert.IsTrue(lastVersion.Diff.Picture.IsChanged, "Picture was changed");
		}

		/// <summary>
		/// Revert to an older version with a different image.
		/// </summary>
		[TestMethod]
		public async Task Revert_ImageChanged()
		{
			// Arrange
			var original = await CallUpdate(ResourceHelper.TestImage()); // First version, this is the one being restored to	
			await CallUpdate(ResourceHelper.TestImage2, "image/png"); // Second version, with a different image

			var entryFromRepo = _repository.Load<Artist>(_artist.Id);
			Assert.IsFalse(PictureData.IsNullOrEmpty(entryFromRepo.Picture), "Artist has picture");
			var oldPictureData = entryFromRepo.Picture.Bytes;

			var oldVer = entryFromRepo.ArchivedVersionsManager.GetVersion(original.Version); // Get the first archived version
			Assert.IsNotNull(oldVer, "Old version was found");

			// Act
			var result = await _queries.RevertToVersion(oldVer.Id);

			// Assert
			entryFromRepo = _repository.Load<Artist>(result.Id);
			Assert.IsTrue(!PictureData.IsNullOrEmpty(entryFromRepo.Picture), "Artist has picture");
			Assert.AreNotEqual(oldPictureData, entryFromRepo.Picture.Bytes, "Picture data was updated");
			Assert.AreEqual(MediaTypeNames.Image.Jpeg, entryFromRepo.PictureMime, "Picture MIME was updated");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			Assert.IsNotNull(lastVersion, "Last version is available");
			Assert.AreEqual(ArtistArchiveReason.Reverted, lastVersion.Reason, "Last version archive reason");
			Assert.IsTrue(lastVersion.Diff.Picture.IsChanged, "Picture was changed");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public async Task Revert_NotAllowed()
		{
			// Regular users can't revert
			_user.GroupId = UserGroupId.Regular;
			_permissionContext.RefreshLoggedUser(_repository);

			await _queries.RevertToVersion(0);
		}

		[TestMethod]
		public async Task Update_Names()
		{
			// Arrange
			var contract = new ArtistForEditContract(_artist, ContentLanguagePreference.English, new InMemoryImagePersister());

			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated artist";

			// Act
			contract = await CallUpdate(contract);

			// Assert
			Assert.AreEqual(_artist.Id, contract.Id, "Update album Id as expected");

			var artistFromRepo = _repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", artistFromRepo.DefaultName);
			Assert.AreEqual(1, artistFromRepo.Version, "Version");

			var archivedVersion = _repository.List<ArchivedArtistVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(_artist, archivedVersion.Artist, "Archived version album");
			Assert.AreEqual(ArtistArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(ArtistEditableFields.Names, archivedVersion.Diff.ChangedFields.Value, "Changed fields");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(_artist, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");
		}

		[TestMethod]
		public async Task Update_OriginalName_UpdateArtistStrings()
		{
			_artist.Names.Names.Clear();
			_artist.Names.Add(new ArtistName(_artist, new LocalizedString("初音ミク", ContentLanguageSelection.Japanese)));
			_artist.Names.Add(new ArtistName(_artist, new LocalizedString("Hatsune Miku", ContentLanguageSelection.Romaji)));
			_artist.TranslatedName.DefaultLanguage = ContentLanguageSelection.Japanese;
			_artist.Names.UpdateSortNames();
			_repository.SaveNames(_artist);

			var song = _repository.Save(CreateEntry.Song());
			_repository.Save(song.AddArtist(_artist));
			song.UpdateArtistString();

			Assert.AreEqual("初音ミク", song.ArtistString[ContentLanguagePreference.Default], "Precondition: default name");

			var contract = new ArtistForEditContract(_artist, ContentLanguagePreference.English, new InMemoryImagePersister());
			contract.DefaultNameLanguage = ContentLanguageSelection.English;

			await CallUpdate(contract);

			Assert.AreEqual("Hatsune Miku", song.ArtistString[ContentLanguagePreference.Default], "Default name was updated");
		}

		[TestMethod]
		public async Task Update_Picture()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());

			var artistFromRepo = _repository.Load(contract.Id);

			Assert.IsFalse(PictureData.IsNullOrEmpty(_artist.Picture), "Picture was saved");
			Assert.AreEqual(MediaTypeNames.Image.Jpeg, artistFromRepo.PictureMime, "Picture.Mime");

			var thumbData = new EntryThumb(artistFromRepo, artistFromRepo.PictureMime, ImagePurpose.Main);
			Assert.IsFalse(_imagePersister.HasImage(thumbData, ImageSize.Original), "Original file was not created"); // Original saved in Picture.Bytes
			Assert.IsTrue(_imagePersister.HasImage(thumbData, ImageSize.Thumb), "Thumbnail file was saved");

			var archivedVersion = _repository.List<ArchivedArtistVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(ArtistEditableFields.Picture, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_ArtistLinks()
		{
			// Arrange
			var circle = _repository.Save(CreateEntry.Circle());
			var illustrator = _repository.Save(CreateEntry.Artist(ArtistType.Illustrator));

			var contract = new ArtistForEditContract(_vocalist, ContentLanguagePreference.English, new InMemoryImagePersister())
			{
				Groups = new[] {
					new ArtistForArtistContract { Parent = new ArtistContract(circle, ContentLanguagePreference.English)},
				},
				Illustrator = new ArtistContract(illustrator, ContentLanguagePreference.English)
			};

			// Act
			await CallUpdate(contract);

			// Assert
			var artistFromRepo = _repository.Load(contract.Id);

			Assert.AreEqual(2, artistFromRepo.AllGroups.Count, "Number of groups");
			Assert.IsTrue(artistFromRepo.HasGroup(circle), "Has group");
			Assert.IsTrue(artistFromRepo.HasGroup(illustrator), "Has illustrator");
			Assert.AreEqual(ArtistLinkType.Group, artistFromRepo.Groups.First(g => g.Parent.Equals(circle)).LinkType, "Artist link type for circle");
			Assert.AreEqual(ArtistLinkType.Illustrator, artistFromRepo.Groups.First(g => g.Parent.Equals(illustrator)).LinkType, "Artist link type for illustrator");
		}

		[TestMethod]
		public async Task Update_ArtistLinks_ChangeRole()
		{
			// Arrange
			var illustrator = _repository.Save(CreateEntry.Artist(ArtistType.Illustrator));
			_vocalist.AddGroup(illustrator, ArtistLinkType.Illustrator);

			// Change linked artist from illustrator to voice provider
			var contract = new ArtistForEditContract(_vocalist, ContentLanguagePreference.English, new InMemoryImagePersister())
			{
				Illustrator = null,
				VoiceProvider = new ArtistContract(illustrator, ContentLanguagePreference.English)
			};

			// Act
			await CallUpdate(contract);

			// Assert
			_vocalist = _repository.Load(contract.Id);

			Assert.AreEqual(1, _vocalist.AllGroups.Count, "Number of linked artists");

			var link = _vocalist.AllGroups[0];
			Assert.AreEqual(illustrator, link.Parent, "Linked artist as expected");
			Assert.AreEqual(ArtistLinkType.VoiceProvider, link.LinkType, "Link type was updated");
		}

		[TestMethod]
		public async Task Update_ArtistLinks_IgnoreInvalid()
		{
			// Arrange
			var circle = _repository.Save(CreateEntry.Circle());
			var circle2 = _repository.Save(CreateEntry.Circle());

			// Cannot add character designer for producer
			var contract = new ArtistForEditContract(_artist, ContentLanguagePreference.English, new InMemoryImagePersister())
			{
				AssociatedArtists = new[] {
					new ArtistForArtistContract(new ArtistForArtist(circle, _artist, ArtistLinkType.CharacterDesigner), ContentLanguagePreference.English),
				},
				Groups = new[] {
					new ArtistForArtistContract(new ArtistForArtist(circle2, _artist, ArtistLinkType.Group), ContentLanguagePreference.English)
				}
			};

			// Act
			await CallUpdate(contract);

			// Assert
			Assert.AreEqual(1, _artist.AllGroups.Count, "Number of linked artists");
			Assert.IsFalse(_artist.HasGroup(circle), "Character designer was not added");
		}
	}
}
