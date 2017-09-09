using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="EventQueries"/>.
	/// </summary>
	[TestClass]
	public class EventQueriesTests {

		private Album album;
		private ReleaseEvent existingEvent;
		private FakeUserMessageMailer mailer = new FakeUserMessageMailer();
		private FakePermissionContext permissionContext;
		private FakeEventRepository repository;
		private EventQueries queries;
		private ReleaseEventSeries series;
		private User user;

		private ReleaseEvent CallUpdate(ReleaseEventForEditContract contract) {
			
			var result = queries.Update(contract, null);
			return repository.Load(result.Id);

		}

		private LocalizedStringWithIdContract[] Names(string name) {
			return new[] { new LocalizedStringWithIdContract(new LocalizedStringWithId(name, ContentLanguageSelection.English)) };
		}

		[TestInitialize]
		public void SetUp() {

			series = CreateEntry.EventSeries("M3");
			existingEvent = CreateEntry.SeriesEvent(series, 2013, "Spring");
			series.AllEvents.Add(existingEvent);

			repository = new FakeEventRepository();
			repository.Save(series);
			repository.Save(existingEvent);
			repository.SaveNames(series);
			repository.SaveNames(existingEvent);

			album = CreateEntry.Album(name: "Day's Footsteps");
			album.OriginalReleaseEvent = existingEvent;
			repository.Save(album);

			user = CreateEntry.User(group: UserGroupId.Trusted);
			repository.Save(user);
			permissionContext = new FakePermissionContext(user);
			queries = new EventQueries(repository, new FakeEntryLinkFactory(), permissionContext, new InMemoryImagePersister(), new FakeUserIconFactory(), new EnumTranslations(), mailer);

		}

		[TestMethod]
		public void Create_NoSeries() {
			
			var contract = new ReleaseEventForEditContract {
				Description = string.Empty,
				Names = Names("Vocaloid Paradise")
			};

			var result = CallUpdate(contract);

			Assert.IsTrue(repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("Vocaloid Paradise", result.DefaultName, "Name");
			Assert.IsNull(result.Series, "Series");

		}

		[TestMethod]
		public void Create_WithSeriesAndSuffix() {
			
			var contract = new ReleaseEventForEditContract {
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(series, ContentLanguagePreference.English),
				SeriesNumber = 2014,
				SeriesSuffix = "Spring",
			};

			var result = CallUpdate(contract);

			Assert.IsTrue(repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("M3 2014 Spring", result.DefaultName, "Name");
			Assert.AreEqual(1, result.Names.Names.Count, "Number of names");
			Assert.AreEqual("M3 2014 Spring", result.Names.Names[0].Value, "First name");
			Assert.AreEqual(2014, result.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Spring", result.SeriesSuffix, "SeriesSuffix");
			Assert.AreSame(series, result.Series, "Series");

		}

		[TestMethod]
		public void Create_WithSeriesNoSuffix() {
			
			var contract = new ReleaseEventForEditContract {
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(series, ContentLanguagePreference.English),
				SeriesNumber = 2014,
				SeriesSuffix = string.Empty,
			};

			var result = CallUpdate(contract);

			Assert.IsTrue(repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("M3 2014", result.DefaultName, "Name");
			Assert.AreEqual(2014, result.SeriesNumber, "SeriesNumber");
			Assert.AreEqual(string.Empty, result.SeriesSuffix, "SeriesSuffix");
			Assert.AreSame(series, result.Series, "Series");

		}

		[TestMethod]
		public void Create_SeriesHasMultipleNames() {

			series.Names.Names = new[] {
				series.CreateName("Comiket", ContentLanguageSelection.English),
				series.CreateName("コミケ", ContentLanguageSelection.Japanese),
				series.CreateName("Comic Market", ContentLanguageSelection.Unspecified),
			}.ToList();

			var contract = new ReleaseEventForEditContract {
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(series, ContentLanguagePreference.English),
				SeriesNumber = 39,
				SeriesSuffix = string.Empty,
			};

			var result = CallUpdate(contract);

			Assert.AreEqual(3, result.Names.Names.Count, "Number of names");
			Assert.IsTrue(result.Names.HasName("Comiket 39"), "Found English name");
			Assert.IsTrue(result.Names.HasName("コミケ 39"), "Found Japanese name");
			Assert.AreEqual("Comiket 39", result.TranslatedName.English, "English name");
			Assert.AreEqual("コミケ 39", result.TranslatedName.Japanese, "Japanese name");

		}

		[TestMethod]
		public void Delete() {
			
			queries.Delete(existingEvent.Id, "Deleted");

			Assert.IsTrue(existingEvent.Deleted, "Deleted");
			var archivedVersion = existingEvent.ArchivedVersionsManager.Versions.FirstOrDefault();
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(EntryEditEvent.Deleted, archivedVersion.EditEvent, "EditEvent");

		}

		[TestMethod]
		public void Update_ChangeSeriesSuffix() {
			
			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, permissionContext, null);
			contract.SeriesSuffix = "Fall";

			var result = CallUpdate(contract);

			Assert.AreEqual(2013, contract.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Fall", contract.SeriesSuffix, "SeriesSuffix");
			Assert.AreEqual("M3 2013 Fall", result.DefaultName, "Name");
			Assert.AreEqual("M3 2013 Fall", album.OriginalReleaseEvent?.DefaultName, "OriginalReleaseEventName for album");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			// Names are changed too when suffix changes
			Assert.AreEqual(ReleaseEventEditableFields.Names | ReleaseEventEditableFields.SeriesSuffix, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		public void Update_ChangeName_CustomName() {

			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, permissionContext, null);
			contract.CustomName = true;
			contract.Names[0].Value = "M3 2013 Fall X2";

			var result = CallUpdate(contract);

			Assert.AreEqual("M3 2013 Fall X2", result.DefaultName, "Name was updated");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Names, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		public void Update_ChangeName_UseSeriesName() {

			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, permissionContext, null);
			contract.Names[0].Value = "New name";
			contract.DefaultNameLanguage = ContentLanguageSelection.Romaji;

			var result = CallUpdate(contract);

			Assert.AreEqual("M3 2013 Spring", result.DefaultName, "Name was not updated");
			Assert.AreEqual(ContentLanguageSelection.English, result.TranslatedName.DefaultLanguage, "Default language was not updated");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Nothing, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void Update_ChangeName_DuplicateOfAnotherEvent() {

			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, permissionContext, null);
			contract.Id = 0; // Simulate new event

			queries.Update(contract, null);

		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void Update_ChangeName_DuplicateForSameEvent() {

			var releaseEvent = repository.Save(CreateEntry.ReleaseEvent("Comiket 39"));
			var contract = new ReleaseEventForEditContract(releaseEvent, ContentLanguagePreference.Default, permissionContext, null) {
				Names = new[] {
					new LocalizedStringWithIdContract {Value = "Comiket 39"},
					new LocalizedStringWithIdContract {Value = "Comiket 39"}
				}
			};

			queries.Update(contract, null);

		}

		[TestMethod]
		public void UpdateSeries_Create() {

			var contract = new ReleaseEventSeriesForEditContract {
				Names = Names("Comiket")
			};

			var result = queries.UpdateSeries(contract, null);

			var seriesFromRepo = repository.Load<ReleaseEventSeries>(result);

			Assert.AreEqual(2, repository.List<ReleaseEventSeries>().Count, "Number of series in repo");
			Assert.IsNotNull(seriesFromRepo, "Series was loaded successfully");
			Assert.AreEqual("Comiket", seriesFromRepo.TranslatedName.Default, "Name was updated");

		}

		/// <summary>
		/// Update series name, event names are updated as well
		/// </summary>
		[TestMethod]
		public void UpdateSeries_UpdateName_EventsUpdated() {

			var contract = new ReleaseEventSeriesForEditContract(series, ContentLanguagePreference.English);
			contract.Names[0].Value = "M3.9";

			var result = queries.UpdateSeries(contract, null);

			var seriesFromRepo = repository.Load<ReleaseEventSeries>(result);

			Assert.AreEqual(1, repository.List<ReleaseEventSeries>().Count, "Number of series in repo");
			Assert.IsNotNull(seriesFromRepo, "Series was loaded successfully");
			Assert.AreEqual("M3.9", seriesFromRepo.TranslatedName.Default, "Name was updated");

			Assert.AreEqual(1, existingEvent.Names.Names.Count, "Number of event names");
			Assert.AreEqual("M3.9 2013 Spring", existingEvent.Names.Names[0].Value, "Event name value");

		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void UpdateSeries_DuplicateName() {

			var series2 = repository.SaveWithNames<ReleaseEventSeries, EventSeriesName>(CreateEntry.EventSeries("M3.9"));
			repository.SaveWithNames<ReleaseEvent, EventName>(CreateEntry.SeriesEvent(series2, 2013, "Spring"));

			var contract = new ReleaseEventSeriesForEditContract(series, ContentLanguagePreference.English);
			contract.Names[0].Value = "M3.9";

			queries.UpdateSeries(contract, null);

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateSeries_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			var contract = new ReleaseEventSeriesForEditContract(series, ContentLanguagePreference.English);
			queries.UpdateSeries(contract, null);

		}

	}

}
