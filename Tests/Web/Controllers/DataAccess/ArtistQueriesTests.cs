using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="ArtistQueries"/>.
	/// </summary>
	[TestClass]
	public class ArtistQueriesTests {
		
		private Artist artist;
		private CreateArtistContract newArtistContract;
		private InMemoryImagePersister imagePersister;
		private FakePermissionContext permissionContext;
		private ArtistQueries queries;
		private FakeArtistRepository repository;
		private User user;

		private ArtistForEditContract CallUpdate(ArtistForEditContract contract) {
			contract.Id = queries.Update(contract, null, permissionContext);
			return contract;
		}

		private ArtistForEditContract CallUpdate(Stream image) {
			var contract = new ArtistForEditContract(artist, ContentLanguagePreference.English);
			using (var stream = image) {
				contract.Id = queries.Update(contract, new EntryPictureFileContract { UploadedFile = stream, Mime = MediaTypeNames.Image.Jpeg }, permissionContext);
			}		
			return contract;
		}

		[TestInitialize]
		public void SetUp() {

			artist = CreateEntry.Producer(name: "Tripshots");
			repository = new FakeArtistRepository(artist);

			foreach (var name in artist.Names)
				repository.Save(name);

			user = CreateEntry.User(name: "Miku");
			repository.Save(user);
			permissionContext = new FakePermissionContext(user);
			imagePersister = new InMemoryImagePersister();

			queries = new ArtistQueries(repository, permissionContext, new FakeEntryLinkFactory(), imagePersister, imagePersister, MemoryCache.Default, 
				new FakeUserIconFactory(), new EnumTranslations());

			newArtistContract = new CreateArtistContract {
				ArtistType = ArtistType.Producer,
				Description = string.Empty,
				Names = new[] {
					new LocalizedStringContract("Tripshots", ContentLanguageSelection.English)
				},
				WebLink = new WebLinkContract("http://tripshots.net/", "Website", WebLinkCategory.Official)
			};

		}

		[TestMethod]
		public void Create() {

			var result = queries.Create(newArtistContract);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Tripshots", result.Name, "Name");

			artist = repository.Load(result.Id);

			Assert.IsNotNull(artist, "Artist was saved to repository");
			Assert.AreEqual("Tripshots", artist.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, artist.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(1, artist.WebLinks.Count, "Weblinks count");

			var archivedVersion = repository.List<ArchivedArtistVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(artist, archivedVersion.Artist, "Archived version artist");
			Assert.AreEqual(ArtistArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(artist, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			queries.Create(newArtistContract);

		}

		[TestMethod]
		public void FindDuplicates_Name() {

			var result = queries.FindDuplicates(new[] { artist.DefaultName }, string.Empty);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Length, "Number of results");
			Assert.AreEqual(artist.Id, result[0].Id, "Matched artist");

		}

		[TestMethod]
		public void FindDuplicates_IgnoreNullsAndEmpty() {

			var result = queries.FindDuplicates(new[] { null, string.Empty }, string.Empty);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(0, result.Length, "Number of results");

		}

		[TestMethod]
		public void GetCoverPictureThumb() {
			
			var contract = CallUpdate(ResourceHelper.TestImage());
			contract.PictureMime = "image/jpeg";

			var result = queries.GetPictureThumb(contract.Id);

			Assert.IsNotNull(result, "result");
			Assert.IsNotNull(result.Picture, "Picture");
			Assert.IsNotNull(result.Picture.Bytes, "Picture content");
			Assert.AreEqual(contract.PictureMime, result.Picture.Mime, "Picture MIME");
			Assert.AreEqual(contract.Id, result.EntryId, "EntryId");

		}

		[TestMethod]
		public void Update_Names() {
			
			var contract = new ArtistForEditContract(artist, ContentLanguagePreference.English);

			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated artist";

			contract = CallUpdate(contract);
			Assert.AreEqual(artist.Id, contract.Id, "Update album Id as expected");

			var artistFromRepo = repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", artistFromRepo.DefaultName);
			Assert.AreEqual(1, artistFromRepo.Version, "Version");

			var archivedVersion = repository.List<ArchivedArtistVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(artist, archivedVersion.Artist, "Archived version album");
			Assert.AreEqual(ArtistArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(ArtistEditableFields.Names, archivedVersion.Diff.ChangedFields, "Changed fields");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(artist, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");

		}

		[TestMethod]
		public void Update_Picture() {

			var contract = CallUpdate(ResourceHelper.TestImage());

			var artistFromRepo = repository.Load(contract.Id);

			Assert.IsNotNull(artistFromRepo.Picture, "Picture");
			Assert.IsNotNull(artistFromRepo.Picture.Bytes, "Original bytes are saved");
			Assert.IsNull(artistFromRepo.Picture.Thumb250, "Thumb bytes not saved anymore");
			Assert.AreEqual(MediaTypeNames.Image.Jpeg, artistFromRepo.PictureMime, "Picture.Mime");

			var thumbData = new EntryThumb(artistFromRepo, artistFromRepo.PictureMime);
			Assert.IsFalse(imagePersister.HasImage(thumbData, ImageSize.Original), "Original file was not created"); // Original saved in Picture.Bytes
			Assert.IsTrue(imagePersister.HasImage(thumbData, ImageSize.Thumb), "Thumbnail file was saved");

			var archivedVersion = repository.List<ArchivedArtistVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(ArtistEditableFields.Picture, archivedVersion.Diff.ChangedFields, "Changed fields");

		}

	}

}
