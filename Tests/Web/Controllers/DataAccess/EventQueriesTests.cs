using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
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
			existingEvent = new ReleaseEvent(string.Empty, null, series, 2013, "Spring", ContentLanguageSelection.Unspecified, null, false);
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
			queries = new EventQueries(repository, new FakeEntryLinkFactory(), permissionContext, new InMemoryImagePersister(), new FakeUserIconFactory(), new EnumTranslations());

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
		public void Update_ChangeSeriesSuffix() {
			
			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, null);
			contract.SeriesSuffix = "Fall";

			var result = CallUpdate(contract);

			Assert.AreEqual(2013, contract.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Fall", contract.SeriesSuffix, "SeriesSuffix");
			Assert.AreEqual("M3 2013 Fall", result.DefaultName, "Name");
			Assert.AreEqual("M3 2013 Fall", album.OriginalReleaseEvent?.DefaultName, "OriginalReleaseEventName for album");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.SeriesSuffix, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		public void Update_ChangeName_CustomName() {

			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, null);
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

			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, null);
			contract.Name = "New name";

			var result = CallUpdate(contract);

			Assert.AreEqual("M3 2013 Spring", result.DefaultName, "Name was not updated");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Nothing, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void Update_ChangeName_Duplicate() {

			var contract = new ReleaseEventForEditContract(existingEvent, ContentLanguagePreference.Default, null);
			contract.Id = 0; // Simulate new event

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

		[TestMethod]
		public void UpdateSeries_Update() {

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
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateSeries_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			var contract = new ReleaseEventSeriesForEditContract(series, ContentLanguagePreference.English);
			queries.UpdateSeries(contract, null);

		}

	}

}
