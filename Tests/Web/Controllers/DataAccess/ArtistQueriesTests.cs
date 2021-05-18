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

			_queries = new ArtistQueries(
				_repository,
				_permissionContext,
				new FakeEntryLinkFactory(),
				_imagePersister,
				_imagePersister,
				MemoryCache.Default,
				new FakeUserIconFactory(),
				new EnumTranslations(),
				_imagePersister,
				new FakeDiscordWebhookNotifier());

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

		private async Task<(bool created, ArtistReport report)> CallCreateReport(ArtistReportType reportType, int? versionNumber = null, Artist artist = null)
		{
			artist ??= _artist;
			var result = await _queries.CreateReport(artist.Id, reportType, "39.39.39.39", "It's Miku, not Rin", versionNumber);
			var report = _repository.Load<ArtistReport>(result.reportId);
			return (result.created, report);
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await _queries.Create(_newArtistContract);

			result.Should().NotBeNull("result");
			result.Name.Should().Be("Tripshots", "Name");

			_artist = _repository.Load(result.Id);

			_artist.Should().NotBeNull("Artist was saved to repository");
			_artist.DefaultName.Should().Be("Tripshots", "Name");
			_artist.Names.SortNames.DefaultLanguage.Should().Be(ContentLanguageSelection.English, "Default language should be English");
			_artist.WebLinks.Count.Should().Be(1, "Weblinks count");

			var archivedVersion = _repository.List<ArchivedArtistVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Artist.Should().Be(_artist, "Archived version artist");
			archivedVersion.Reason.Should().Be(ArtistArchiveReason.Created, "Archived version reason");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			activityEntry.Should().NotBeNull("Activity entry was created");
			activityEntry.EntryBase.Should().Be(_artist, "Activity entry's entry");
			activityEntry.EditEvent.Should().Be(EntryEditEvent.Created, "Activity entry event type");
		}

		[TestMethod]
		public void Create_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			_queries.Awaiting(subject => subject.Create(_newArtistContract)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public async Task CreateReport()
		{
			var editor = _user2;
			_repository.Save(ArchivedArtistVersion.Create(_artist, new ArtistDiff(), new AgentLoginData(editor), ArtistArchiveReason.PropertiesUpdated, string.Empty));
			var (created, report) = await CallCreateReport(ArtistReportType.InvalidInfo);

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
		public async Task CreateReport_OwnershipClaim()
		{
			var editor = _user2;
			_repository.Save(ArchivedArtistVersion.Create(_artist, new ArtistDiff(), new AgentLoginData(editor), ArtistArchiveReason.PropertiesUpdated, string.Empty));
			var (created, _) = await CallCreateReport(ArtistReportType.OwnershipClaim);

			created.Should().BeTrue("Report was created");

			_repository.List<UserMessage>().Should().BeEmpty("No notification created");
		}

		[TestMethod]
		public void FindDuplicates_Name()
		{
			var result = _queries.FindDuplicates(new[] { _artist.DefaultName }, string.Empty);

			result.Should().NotBeNull("result");
			result.Length.Should().Be(1, "Number of results");
			result[0].Id.Should().Be(_artist.Id, "Matched artist");
		}

		[TestMethod]
		public void FindDuplicates_Link()
		{
			var result = _queries.FindDuplicates(new string[0], "http://tripshots.net");

			result.Should().NotBeNull("result");
			result.Length.Should().Be(1, "Number of results");
			result[0].Id.Should().Be(_artist.Id, "Matched artist");
		}

		[TestMethod]
		public void FindDuplicates_DifferentScheme()
		{
			var result = _queries.FindDuplicates(new string[0], "https://tripshots.net");

			result.Should().NotBeNull("result");
			result.Length.Should().Be(1, "Number of results");
			result[0].Id.Should().Be(_artist.Id, "Matched artist");
		}

		[TestMethod]
		public void FindDuplicates_IgnoreNullsAndEmpty()
		{
			var result = _queries.FindDuplicates(new[] { null, string.Empty }, string.Empty);

			result.Should().NotBeNull("result");
			result.Length.Should().Be(0, "Number of results");
		}

		[TestMethod]
		public void FindDuplicates_Link_IgnoreDeleted()
		{
			_artist.Deleted = true;
			var result = _queries.FindDuplicates(new string[0], "http://tripshots.net");

			result.Should().NotBeNull("result");
			result.Length.Should().Be(0, "Number of results");
		}

		[TestMethod]
		public void FindDuplicates_Link_IgnoreInvalidLink()
		{
			var result = _queries.FindDuplicates(new string[0], "Miku!");
			result?.Length.Should().Be(0, "Number of results");
		}

		[TestMethod]
		public async Task GetCoverPictureThumb()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());
			contract.PictureMime = "image/jpeg";

			var result = _queries.GetPictureThumb(contract.Id);

			result.Should().NotBeNull("result");
			result.Picture.Should().NotBeNull("Picture");
			result.Picture.Bytes.Should().NotBeNull("Picture content");
			result.Picture.Mime.Should().Be(contract.PictureMime, "Picture MIME");
			result.EntryId.Should().Be(contract.Id, "EntryId");
		}

		[TestMethod]
		public void GetDetails()
		{
			var result = _queries.GetDetails(_artist.Id, "39.39.39.39");

			result.Name.Should().Be("Tripshots", "Name");

			var hit = _repository.List<ArtistHit>().FirstOrDefault(a => a.Entry.Equals(_artist));
			hit.Should().NotBeNull("Hit was created");
			hit.Agent.Should().Be(_user.Id, "Hit creator");
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
			entryFromRepo.Description.English.Should().Be("Updated", "Description was updated");

			// Act
			var result = await _queries.RevertToVersion(oldVer.Id);

			// Assert
			result.Warnings.Length.Should().Be(0, "Number of warnings");

			entryFromRepo = _repository.Load<Artist>(result.Id);
			entryFromRepo.Description.English.Should().Be("Original", "Description was restored");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			lastVersion.Should().NotBeNull("Last version is available");
			lastVersion.Reason.Should().Be(ArtistArchiveReason.Reverted, "Last version archive reason");
			lastVersion.Diff.Picture.IsChanged.Should().BeFalse("Picture was not changed");
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
			PictureData.IsNullOrEmpty(entryFromRepo.Picture).Should().BeTrue("Picture data was removed");
			string.IsNullOrEmpty(entryFromRepo.PictureMime).Should().BeTrue("Picture MIME was removed");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			lastVersion.Should().NotBeNull("Last version is available");
			lastVersion.Version.Should().Be(2, "Last version number");
			lastVersion.Reason.Should().Be(ArtistArchiveReason.Reverted, "Last version archive reason");
			lastVersion.Diff.Picture.IsChanged.Should().BeTrue("Picture was changed");
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
			PictureData.IsNullOrEmpty(entryFromRepo.Picture).Should().BeFalse("Artist has picture");
			var oldPictureData = entryFromRepo.Picture.Bytes;

			var oldVer = entryFromRepo.ArchivedVersionsManager.GetVersion(original.Version); // Get the first archived version
			oldVer.Should().NotBeNull("Old version was found");

			// Act
			var result = await _queries.RevertToVersion(oldVer.Id);

			// Assert
			entryFromRepo = _repository.Load<Artist>(result.Id);
			PictureData.IsNullOrEmpty(entryFromRepo.Picture).Should().BeFalse("Artist has picture");
			entryFromRepo.Picture.Bytes.Should().NotEqual(oldPictureData, "Picture data was updated");
			entryFromRepo.PictureMime.Should().Be(MediaTypeNames.Image.Jpeg, "Picture MIME was updated");

			var lastVersion = entryFromRepo.ArchivedVersionsManager.GetLatestVersion();
			lastVersion.Should().NotBeNull("Last version is available");
			lastVersion.Reason.Should().Be(ArtistArchiveReason.Reverted, "Last version archive reason");
			lastVersion.Diff.Picture.IsChanged.Should().BeTrue("Picture was changed");
		}

		[TestMethod]
		public void Revert_NotAllowed()
		{
			// Regular users can't revert
			_user.GroupId = UserGroupId.Regular;
			_permissionContext.RefreshLoggedUser(_repository);

			_queries.Awaiting(subject => subject.RevertToVersion(0)).Should().Throw<NotAllowedException>();
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
			contract.Id.Should().Be(_artist.Id, "Update album Id as expected");

			var artistFromRepo = _repository.Load(contract.Id);
			artistFromRepo.DefaultName.Should().Be("Replaced name");
			artistFromRepo.Version.Should().Be(1, "Version");

			var archivedVersion = _repository.List<ArchivedArtistVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Artist.Should().Be(_artist, "Archived version album");
			archivedVersion.Reason.Should().Be(ArtistArchiveReason.PropertiesUpdated, "Archived version reason");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(ArtistEditableFields.Names, "Changed fields");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			activityEntry.Should().NotBeNull("Activity entry was created");
			activityEntry.EntryBase.Should().Be(_artist, "Activity entry's entry");
			activityEntry.EditEvent.Should().Be(EntryEditEvent.Updated, "Activity entry event type");
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

			song.ArtistString[ContentLanguagePreference.Default].Should().Be("初音ミク", "Precondition: default name");

			var contract = new ArtistForEditContract(_artist, ContentLanguagePreference.English, new InMemoryImagePersister());
			contract.DefaultNameLanguage = ContentLanguageSelection.English;

			await CallUpdate(contract);

			song.ArtistString[ContentLanguagePreference.Default].Should().Be("Hatsune Miku", "Default name was updated");
		}

		[TestMethod]
		public async Task Update_Picture()
		{
			var contract = await CallUpdate(ResourceHelper.TestImage());

			var artistFromRepo = _repository.Load(contract.Id);

			PictureData.IsNullOrEmpty(_artist.Picture).Should().BeFalse("Picture was saved");
			artistFromRepo.PictureMime.Should().Be(MediaTypeNames.Image.Jpeg, "Picture.Mime");

			var thumbData = new EntryThumb(artistFromRepo, artistFromRepo.PictureMime, ImagePurpose.Main);
			_imagePersister.HasImage(thumbData, ImageSize.Original).Should().BeFalse("Original file was not created"); // Original saved in Picture.Bytes
			_imagePersister.HasImage(thumbData, ImageSize.Thumb).Should().BeTrue("Thumbnail file was saved");

			var archivedVersion = _repository.List<ArchivedArtistVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(ArtistEditableFields.Picture, "Changed fields");
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

			artistFromRepo.AllGroups.Count.Should().Be(2, "Number of groups");
			artistFromRepo.HasGroup(circle).Should().BeTrue("Has group");
			artistFromRepo.HasGroup(illustrator).Should().BeTrue("Has illustrator");
			artistFromRepo.Groups.First(g => g.Parent.Equals(circle)).LinkType.Should().Be(ArtistLinkType.Group, "Artist link type for circle");
			artistFromRepo.Groups.First(g => g.Parent.Equals(illustrator)).LinkType.Should().Be(ArtistLinkType.Illustrator, "Artist link type for illustrator");
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

			_vocalist.AllGroups.Count.Should().Be(1, "Number of linked artists");

			var link = _vocalist.AllGroups[0];
			link.Parent.Should().Be(illustrator, "Linked artist as expected");
			link.LinkType.Should().Be(ArtistLinkType.VoiceProvider, "Link type was updated");
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
			_artist.AllGroups.Count.Should().Be(1, "Number of linked artists");
			_artist.HasGroup(circle).Should().BeFalse("Character designer was not added");
		}
	}
}
