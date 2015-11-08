using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
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
		private FakeEventRepository repository;
		private EventQueries queries;
		private ReleaseEventSeries series;

		private ReleaseEvent CallUpdate(ReleaseEventDetailsContract contract) {
			
			var result = queries.Update(contract);
			return repository.Load(result.Id);

		}

		[TestInitialize]
		public void SetUp() {

			series = CreateEntry.EventSeries("M3");
			existingEvent = new ReleaseEvent(string.Empty, null, series, 2013, "Spring");

			repository = new FakeEventRepository();
			repository.Save(series);
			repository.Save(existingEvent);

			album = CreateEntry.Album(name: "Day's Footsteps");
			album.OriginalReleaseEventName = "M3 2013 Spring";
			repository.Save(album);

			var user = CreateEntry.User();
			repository.Save(user);
			queries = new EventQueries(repository, new FakeEntryLinkFactory(), new FakePermissionContext(user));

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

		}

	}

}
