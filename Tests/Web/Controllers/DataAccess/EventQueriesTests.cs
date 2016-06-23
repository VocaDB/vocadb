using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

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

		private ReleaseEvent CallUpdate(ReleaseEventDetailsContract contract) {
			
			var result = queries.Update(contract);
			return repository.Load(result.Id);

		}

		[TestInitialize]
		public void SetUp() {

			series = CreateEntry.EventSeries("M3");
			existingEvent = new ReleaseEvent(string.Empty, null, series, 2013, "Spring", null, false);

			repository = new FakeEventRepository();
			repository.Save(series);
			repository.Save(existingEvent);

			album = CreateEntry.Album(name: "Day's Footsteps");
			album.OriginalReleaseEventName = "M3 2013 Spring";
			repository.Save(album);

			user = CreateEntry.User(group: UserGroupId.Trusted);
			repository.Save(user);
			permissionContext = new FakePermissionContext(user);
			queries = new EventQueries(repository, new FakeEntryLinkFactory(), permissionContext, new InMemoryImagePersister());

		}

		[TestMethod]
		public void Create_NoSeries() {
			
			var contract = new ReleaseEventDetailsContract {
				Description = string.Empty,
				Name = "Vocaloid Paradise",
			};

			var result = CallUpdate(contract);

			Assert.IsTrue(repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("Vocaloid Paradise", result.Name, "Name");
			Assert.IsNull(result.Series, "Series");

		}

		[TestMethod]
		public void Create_WithSeriesAndSuffix() {

			
			var contract = new ReleaseEventDetailsContract {
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(series),
				SeriesNumber = 2014,
				SeriesSuffix = "Spring",
			};

			var result = CallUpdate(contract);

			Assert.IsTrue(repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("M3 2014 Spring", result.Name, "Name");
			Assert.AreEqual(2014, result.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Spring", result.SeriesSuffix, "SeriesSuffix");
			Assert.AreSame(series, result.Series, "Series");

		}

		[TestMethod]
		public void Create_WithSeriesNoSuffix() {

			
			var contract = new ReleaseEventDetailsContract {
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(series),
				SeriesNumber = 2014,
				SeriesSuffix = string.Empty,
			};

			var result = CallUpdate(contract);

			Assert.IsTrue(repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("M3 2014", result.Name, "Name");
			Assert.AreEqual(2014, result.SeriesNumber, "SeriesNumber");
			Assert.AreEqual(string.Empty, result.SeriesSuffix, "SeriesSuffix");
			Assert.AreSame(series, result.Series, "Series");

		}

		[TestMethod]
		public void Update_ChangeSeriesSuffix() {
			
			var contract = new ReleaseEventDetailsContract(existingEvent, ContentLanguagePreference.Default);
			contract.SeriesSuffix = "Fall";

			var result = CallUpdate(contract);

			Assert.AreEqual(2013, contract.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Fall", contract.SeriesSuffix, "SeriesSuffix");
			Assert.AreEqual("M3 2013 Fall", result.Name, "Name");
			Assert.AreEqual("M3 2013 Fall", album.OriginalReleaseEventName, "OriginalReleaseEventName for album");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.SeriesSuffix, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		public void Update_ChangeName_CustomName() {

			var contract = new ReleaseEventDetailsContract(existingEvent, ContentLanguagePreference.Default);
			contract.CustomName = true;
			contract.Name = "M3 2013 Fall X2";

			var result = CallUpdate(contract);

			Assert.AreEqual("M3 2013 Fall X2", result.Name, "Name was updated");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Name, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		public void Update_ChangeName_UseSeriesName() {

			var contract = new ReleaseEventDetailsContract(existingEvent, ContentLanguagePreference.Default);
			contract.Name = "New name";

			var result = CallUpdate(contract);

			Assert.AreEqual("M3 2013 Spring", result.Name, "Name was not updated");

			var archivedVersions = repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Nothing, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");

		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void Update_ChangeName_Duplicate() {

			var contract = new ReleaseEventDetailsContract(existingEvent, ContentLanguagePreference.Default);
			contract.Id = 0; // Simulate new event

			queries.Update(contract);

		}

		[TestMethod]
		public void UpdateSeries_Create() {

			var contract = new ReleaseEventSeriesForEditContract { Name = "Comiket" };

			var result = queries.UpdateSeries(contract, null);

			var seriesFromRepo = repository.Load<ReleaseEventSeries>(result);

			Assert.AreEqual(2, repository.List<ReleaseEventSeries>().Count, "Number of series in repo");
			Assert.IsNotNull(seriesFromRepo, "Series was loaded successfully");
			Assert.AreEqual("Comiket", seriesFromRepo.Name, "Name was updated");

		}

		[TestMethod]
		public void UpdateSeries_Update() {

			var contract = new ReleaseEventSeriesForEditContract(series);
			contract.Name = "M3.9";

			var result = queries.UpdateSeries(contract, null);

			var seriesFromRepo = repository.Load<ReleaseEventSeries>(result);

			Assert.AreEqual(1, repository.List<ReleaseEventSeries>().Count, "Number of series in repo");
			Assert.IsNotNull(seriesFromRepo, "Series was loaded successfully");
			Assert.AreEqual("M3.9", seriesFromRepo.Name, "Name was updated");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateSeries_NoPermission() {

			user.GroupId = UserGroupId.Regular;
			permissionContext.RefreshLoggedUser(repository);

			var contract = new ReleaseEventSeriesForEditContract(series);
			queries.UpdateSeries(contract, null);

		}

	}

}
