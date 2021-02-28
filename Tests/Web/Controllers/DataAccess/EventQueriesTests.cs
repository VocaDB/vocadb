#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

			_repository.Contains(result).Should().BeTrue("Event was saved to repository");
			result.DefaultName.Should().Be("Vocaloid Paradise", "Name");
			result.Series.Should().BeNull("Series");
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

			_repository.Contains(result).Should().BeTrue("Event was saved to repository");
			result.DefaultName.Should().Be("M3 2014 Spring", "Name");
			result.Names.Names.Count.Should().Be(1, "Number of names");
			result.Names.Names[0].Value.Should().Be("M3 2014 Spring", "First name");
			result.SeriesNumber.Should().Be(2014, "SeriesNumber");
			result.SeriesSuffix.Should().Be("Spring", "SeriesSuffix");
			result.Series.Should().BeSameAs(_series, "Series");
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

			_repository.Contains(result).Should().BeTrue("Event was saved to repository");
			result.DefaultName.Should().Be("M3 2014", "Name");
			result.SeriesNumber.Should().Be(2014, "SeriesNumber");
			result.SeriesSuffix.Should().Be(string.Empty, "SeriesSuffix");
			result.Series.Should().BeSameAs(_series, "Series");
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

			result.Names.Names.Count.Should().Be(3, "Number of names");
			result.Names.HasName("Comiket 39").Should().BeTrue("Found English name");
			result.Names.HasName("コミケ 39").Should().BeTrue("Found Japanese name");
			result.TranslatedName.English.Should().Be("Comiket 39", "English name");
			result.TranslatedName.Japanese.Should().Be("コミケ 39", "Japanese name");
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

			_existingEvent.Deleted.Should().BeTrue("Deleted");
			var archivedVersion = _existingEvent.ArchivedVersionsManager.Versions.FirstOrDefault();
			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.EditEvent.Should().Be(EntryEditEvent.Deleted, "EditEvent");
		}

		[TestMethod]
		public async Task Update_ChangeSeriesSuffix()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.SeriesSuffix = "Fall";

			var result = await CallUpdate(contract);

			contract.SeriesNumber.Should().Be(2013, "SeriesNumber");
			contract.SeriesSuffix.Should().Be("Fall", "SeriesSuffix");
			result.DefaultName.Should().Be("M3 2013 Fall", "Name");
			_album.OriginalReleaseEvent?.DefaultName.Should().Be("M3 2013 Fall", "OriginalReleaseEventName for album");

			var archivedVersions = _repository.List<ArchivedReleaseEventVersion>();
			archivedVersions.Count.Should().Be(1, "Archived version was created");
			// Names are changed too when suffix changes
			archivedVersions[0].Diff.ChangedFields.Value.Should().Be(ReleaseEventEditableFields.Names | ReleaseEventEditableFields.SeriesSuffix, "Changed fields in diff");
		}

		[TestMethod]
		public async Task Update_ChangeName_CustomName()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.CustomName = true;
			contract.Names[0].Value = "M3 2013 Fall X2";

			var result = await CallUpdate(contract);

			result.DefaultName.Should().Be("M3 2013 Fall X2", "Name was updated");

			var archivedVersions = _repository.List<ArchivedReleaseEventVersion>();
			archivedVersions.Count.Should().Be(1, "Archived version was created");
			archivedVersions[0].Diff.ChangedFields.Value.Should().Be(ReleaseEventEditableFields.Names, "Changed fields in diff");
		}

		[TestMethod]
		public async Task Update_ChangeName_UseSeriesName()
		{
			var contract = new ReleaseEventForEditContract(_existingEvent, ContentLanguagePreference.Default, _permissionContext, null);
			contract.Names[0].Value = "New name";
			contract.DefaultNameLanguage = ContentLanguageSelection.Romaji;

			var result = await CallUpdate(contract);

			result.DefaultName.Should().Be("M3 2013 Spring", "Name was not updated");
			result.TranslatedName.DefaultLanguage.Should().Be(ContentLanguageSelection.English, "Default language was not updated");

			var archivedVersions = _repository.List<ArchivedReleaseEventVersion>();
			archivedVersions.Count.Should().Be(1, "Archived version was created");
			archivedVersions[0].Diff.ChangedFields.Value.Should().Be(ReleaseEventEditableFields.Nothing, "Changed fields in diff");
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

			releaseEvent.Series.Should().Be(_series, "Series");
			_series.AllEvents.Contains(releaseEvent).Should().BeTrue("Series contains event");
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

			_repository.List<ReleaseEventSeries>().Count.Should().Be(2, "Number of series in repo");
			seriesFromRepo.Should().NotBeNull("Series was loaded successfully");
			seriesFromRepo.TranslatedName.Default.Should().Be("Comiket", "Name was updated");
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

			_repository.List<ReleaseEventSeries>().Count.Should().Be(1, "Number of series in repo");
			seriesFromRepo.Should().NotBeNull("Series was loaded successfully");
			seriesFromRepo.TranslatedName.Default.Should().Be("M3.9", "Name was updated");

			_existingEvent.Names.Names.Count.Should().Be(1, "Number of event names");
			_existingEvent.Names.Names[0].Value.Should().Be("M3.9 2013 Spring", "Event name value");
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
			_existingEvent.TranslatedName.DefaultLanguage.Should().Be(ContentLanguageSelection.Japanese, "Default language");
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
