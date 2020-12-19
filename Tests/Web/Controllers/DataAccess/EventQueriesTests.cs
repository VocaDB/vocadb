#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="EventQueries"/>.
	/// </summary>
	[TestClass]
	public class EventQueriesTests
	{
		private Album _album;
		private ReleaseEvent _existingEvent;
		private readonly FakeUserMessageMailer _mailer = new();
		private FakePermissionContext _permissionContext;
		private FakeEventRepository _repository;
		private EventQueries _queries;
		private ReleaseEventSeries _series;
		private User _user;

		private async Task<ReleaseEvent> CallUpdate(ReleaseEventForEditContract contract)
		{
			var result = await _queries.Update(contract, null);
			return _repository.Load(result.Id);
		}

		private ReleaseEventForEditContract Contract(ReleaseEvent releaseEvent)
		{
			return new ReleaseEventForEditContract(releaseEvent, ContentLanguagePreference.Default, _permissionContext, null);
		}

		private LocalizedStringWithIdContract[] Names(string name)
		{
			return new[] { new LocalizedStringWithIdContract(new LocalizedStringWithId(name, ContentLanguageSelection.English)) };
		}

		[TestInitialize]
		public void SetUp()
		{
			_series = CreateEntry.EventSeries("M3");
			_existingEvent = CreateEntry.SeriesEvent(_series, 2013, "Spring");
			_series.AllEvents.Add(_existingEvent);

			_repository = new FakeEventRepository();
			_repository.Save(_series);
			_repository.Save(_existingEvent);
			_repository.SaveNames(_series);
			_repository.SaveNames(_existingEvent);

			_album = CreateEntry.Album(name: "Day's Footsteps");
			_album.OriginalReleaseEvent = _existingEvent;
			_repository.Save(_album);

			_user = CreateEntry.User(group: UserGroupId.Trusted);
			_repository.Save(_user);
			_permissionContext = new FakePermissionContext(_user);
			var imageStore = new InMemoryImagePersister();
			_queries = new EventQueries(_repository, new FakeEntryLinkFactory(), _permissionContext, imageStore, new FakeUserIconFactory(), new EnumTranslations(), _mailer,
				new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()), imageStore);
		}

		[TestMethod]
		public async Task Create_NoSeries()
		{
			var contract = new ReleaseEventForEditContract
			{
				Description = string.Empty,
				Names = Names("Vocaloid Paradise")
			};

			var result = await CallUpdate(contract);

			Assert.IsTrue(_repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("Vocaloid Paradise", result.DefaultName, "Name");
			Assert.IsNull(result.Series, "Series");
		}

		[TestMethod]
		public async Task Create_WithSeriesAndSuffix()
		{
			var contract = new ReleaseEventForEditContract
			{
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(_series, ContentLanguagePreference.English),
				SeriesNumber = 2014,
				SeriesSuffix = "Spring",
			};

			var result = await CallUpdate(contract);

			Assert.IsTrue(_repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("M3 2014 Spring", result.DefaultName, "Name");
			Assert.AreEqual(1, result.Names.Names.Count, "Number of names");
			Assert.AreEqual("M3 2014 Spring", result.Names.Names[0].Value, "First name");
			Assert.AreEqual(2014, result.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Spring", result.SeriesSuffix, "SeriesSuffix");
			Assert.AreSame(_series, result.Series, "Series");
		}

		[TestMethod]
		public async Task Create_WithSeriesNoSuffix()
		{
			var contract = new ReleaseEventForEditContract
			{
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(_series, ContentLanguagePreference.English),
				SeriesNumber = 2014,
				SeriesSuffix = string.Empty,
			};

			var result = await CallUpdate(contract);

			Assert.IsTrue(_repository.Contains(result), "Event was saved to repository");
			Assert.AreEqual("M3 2014", result.DefaultName, "Name");
			Assert.AreEqual(2014, result.SeriesNumber, "SeriesNumber");
			Assert.AreEqual(string.Empty, result.SeriesSuffix, "SeriesSuffix");
			Assert.AreSame(_series, result.Series, "Series");
		}

		[TestMethod]
		public async Task Create_SeriesHasMultipleNames()
		{
			_series.Names.Names = new[] {
				_series.CreateName("Comiket", ContentLanguageSelection.English),
				_series.CreateName("コミケ", ContentLanguageSelection.Japanese),
				_series.CreateName("Comic Market", ContentLanguageSelection.Unspecified),
			}.ToList();

			var contract = new ReleaseEventForEditContract
			{
				Description = string.Empty,
				Series = new ReleaseEventSeriesContract(_series, ContentLanguagePreference.English),
				SeriesNumber = 39,
				SeriesSuffix = string.Empty,
			};

			var result = await CallUpdate(contract);

			Assert.AreEqual(3, result.Names.Names.Count, "Number of names");
			Assert.IsTrue(result.Names.HasName("Comiket 39"), "Found English name");
			Assert.IsTrue(result.Names.HasName("コミケ 39"), "Found Japanese name");
			Assert.AreEqual("Comiket 39", result.TranslatedName.English, "English name");
			Assert.AreEqual("コミケ 39", result.TranslatedName.Japanese, "Japanese name");
		}

		[TestMethod]
		public async Task Create_WithCustomArtists()
		{
			var artist = _repository.Save(CreateEntry.Artist(ArtistType.Producer));
			var contract = new ReleaseEventForEditContract
			{
				Description = string.Empty,
				SeriesSuffix = string.Empty,
				Artists = new[] {
					new ArtistForEventContract { Artist = new ArtistContract(artist, ContentLanguagePreference.Default) },
					new ArtistForEventContract { Name = "Miku!" }
				}
			};

			await CallUpdate(contract);
		}

		[TestMethod]
		public void Delete()
		{
			_queries.Delete(_existingEvent.Id, "Deleted");

			Assert.IsTrue(_existingEvent.Deleted, "Deleted");
			var archivedVersion = _existingEvent.ArchivedVersionsManager.Versions.FirstOrDefault();
			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(EntryEditEvent.Deleted, archivedVersion.EditEvent, "EditEvent");
		}

		[TestMethod]
		public async Task Update_ChangeSeriesSuffix()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.SeriesSuffix = "Fall";

			var result = await CallUpdate(contract);

			Assert.AreEqual(2013, contract.SeriesNumber, "SeriesNumber");
			Assert.AreEqual("Fall", contract.SeriesSuffix, "SeriesSuffix");
			Assert.AreEqual("M3 2013 Fall", result.DefaultName, "Name");
			Assert.AreEqual("M3 2013 Fall", _album.OriginalReleaseEvent?.DefaultName, "OriginalReleaseEventName for album");

			var archivedVersions = _repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			// Names are changed too when suffix changes
			Assert.AreEqual(ReleaseEventEditableFields.Names | ReleaseEventEditableFields.SeriesSuffix, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");
		}

		[TestMethod]
		public async Task Update_ChangeName_CustomName()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.CustomName = true;
			contract.Names[0].Value = "M3 2013 Fall X2";

			var result = await CallUpdate(contract);

			Assert.AreEqual("M3 2013 Fall X2", result.DefaultName, "Name was updated");

			var archivedVersions = _repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Names, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");
		}

		[TestMethod]
		public async Task Update_ChangeName_UseSeriesName()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.Names[0].Value = "New name";
			contract.DefaultNameLanguage = ContentLanguageSelection.Romaji;

			var result = await CallUpdate(contract);

			Assert.AreEqual("M3 2013 Spring", result.DefaultName, "Name was not updated");
			Assert.AreEqual(ContentLanguageSelection.English, result.TranslatedName.DefaultLanguage, "Default language was not updated");

			var archivedVersions = _repository.List<ArchivedReleaseEventVersion>();
			Assert.AreEqual(1, archivedVersions.Count, "Archived version was created");
			Assert.AreEqual(ReleaseEventEditableFields.Nothing, archivedVersions[0].Diff.ChangedFields.Value, "Changed fields in diff");
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public async Task Update_ChangeName_DuplicateOfAnotherEvent()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.Id = 0; // Simulate new event

			await _queries.Update(contract, null);
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public async Task Update_ChangeName_DuplicateForSameEvent()
		{
			var releaseEvent = _repository.Save(CreateEntry.ReleaseEvent("Comiket 39"));
			var contract = new ReleaseEventForEditContract(releaseEvent, ContentLanguagePreference.Default, _permissionContext, null)
			{
				Names = new[] {
					new LocalizedStringWithIdContract {Value = "Comiket 39"},
					new LocalizedStringWithIdContract {Value = "Comiket 39"}
				}
			};

			await _queries.Update(contract, null);
		}

		[TestMethod]
		public async Task Update_ChangeToSeriesEvent()
		{
			var releaseEvent = _repository.Save(CreateEntry.ReleaseEvent("M3 39"));
			var contract = Contract(releaseEvent);
			contract.Series = new ReleaseEventSeriesContract(_series, ContentLanguagePreference.Default);

			await _queries.Update(contract, null);

			Assert.AreEqual(_series, releaseEvent.Series, "Series");
			Assert.IsTrue(_series.AllEvents.Contains(releaseEvent), "Series contains event");
		}

		[TestMethod]
		public void UpdateSeries_Create()
		{
			var contract = new ReleaseEventSeriesForEditContract
			{
				Names = Names("Comiket")
			};

			var result = _queries.UpdateSeries(contract, null);

			var seriesFromRepo = _repository.Load<ReleaseEventSeries>(result);

			Assert.AreEqual(2, _repository.List<ReleaseEventSeries>().Count, "Number of series in repo");
			Assert.IsNotNull(seriesFromRepo, "Series was loaded successfully");
			Assert.AreEqual("Comiket", seriesFromRepo.TranslatedName.Default, "Name was updated");
		}

		/// <summary>
		/// Update series name, event names are updated as well
		/// </summary>
		[TestMethod]
		public void UpdateSeries_UpdateName_EventsUpdated()
		{
			var contract = new ReleaseEventSeriesForEditContract(_series, ContentLanguagePreference.English);
			contract.Names[0].Value = "M3.9";

			var result = _queries.UpdateSeries(contract, null);

			var seriesFromRepo = _repository.Load<ReleaseEventSeries>(result);

			Assert.AreEqual(1, _repository.List<ReleaseEventSeries>().Count, "Number of series in repo");
			Assert.IsNotNull(seriesFromRepo, "Series was loaded successfully");
			Assert.AreEqual("M3.9", seriesFromRepo.TranslatedName.Default, "Name was updated");

			Assert.AreEqual(1, _existingEvent.Names.Names.Count, "Number of event names");
			Assert.AreEqual("M3.9 2013 Spring", _existingEvent.Names.Names[0].Value, "Event name value");
		}

		/// <summary>
		/// Updates series default language selection, inherited language to events is updated as well.
		/// </summary>
		[TestMethod]
		public void UpdateSeries_UpdateDefaultLanguage_EventsUpdated()
		{
			var contract = new ReleaseEventSeriesForEditContract(_series, ContentLanguagePreference.English)
			{
				DefaultNameLanguage = ContentLanguageSelection.Japanese
			};

			var result = _queries.UpdateSeries(contract, null);
			Assert.AreEqual(ContentLanguageSelection.Japanese, _existingEvent.TranslatedName.DefaultLanguage, "Default language");
		}

		[TestMethod]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void UpdateSeries_DuplicateName()
		{
			var series2 = _repository.SaveWithNames<ReleaseEventSeries, EventSeriesName>(CreateEntry.EventSeries("M3.9"));
			_repository.SaveWithNames<ReleaseEvent, EventName>(CreateEntry.SeriesEvent(series2, 2013, "Spring"));

			var contract = new ReleaseEventSeriesForEditContract(_series, ContentLanguagePreference.English);
			contract.Names[0].Value = "M3.9";

			_queries.UpdateSeries(contract, null);
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateSeries_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new ReleaseEventSeriesForEditContract(_series, ContentLanguagePreference.English);
			_queries.UpdateSeries(contract, null);
		}
	}
}
