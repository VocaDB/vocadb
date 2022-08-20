using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue1191;

[TestClass]
public class SongQueriesTests
{
	private EntryAnchorFactory _entryLinkFactory = default!;
	private FakeUserMessageMailer _mailer = default!;
	private FakePermissionContext _permissionContext = default!;
	private FakePVParser _pvParser = default!;
	private FakeSongRepository _repository = default!;
	private SongQueries _queries = default!;
	private ReleaseEvent _releaseEvent = default!;
	private Song _song = default!;
	private User _user = default!;
	private User _user2 = default!;

	[TestInitialize]
	public void SetUp()
	{
		_song = CreateEntry.Song(id: 1, name: "Nebula");
		_song.LengthSeconds = 39;
		_repository = new FakeSongRepository(_song);
		_repository.SaveNames(_song);

		_user = CreateEntry.User(id: 1, name: "Miku");
		_user.GroupId = UserGroupId.Trusted;
		_user2 = CreateEntry.User(id: 2, name: "Rin", email: "rin@vocadb.net");
		_repository.Add(_user, _user2);

		_releaseEvent = _repository.Save(CreateEntry.ReleaseEvent("Comiket 39"));

		_permissionContext = new FakePermissionContext(_user);
		_entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

		_pvParser = new()
		{
			ResultFunc = (url, getMeta) =>
			VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939",
			getMeta ? VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", null, "testimg.jpg", 39) : VideoTitleParseResult.Empty)
		};

		_mailer = new FakeUserMessageMailer();

		_queries = new SongQueries(
			_repository,
			_permissionContext,
			_entryLinkFactory,
			_pvParser,
			_mailer,
			new FakeLanguageDetector(),
			new FakeUserIconFactory(),
			new EnumTranslations(),
			new InMemoryImagePersister(),
			new FakeObjectCache(),
			new Model.Utils.Config.VdbConfigManager(),
			new EntrySubTypeNameFactory(),
			new FollowedArtistNotifier(new FakeEntryLinkFactory(), _mailer, new EnumTranslations(), new EntrySubTypeNameFactory()),
			new FakeDiscordWebhookNotifier()
		);
	}

	private SongForEditForApiContract EditContract()
	{
		return new SongForEditForApiContract(_song, ContentLanguagePreference.English, _permissionContext);
	}

	[TestMethod]
	public async Task Merge_ToEmpty()
	{
		var song2 = new Song();
		_repository.Save(song2);

		var contract = EditContract();
		contract.ReleaseEvent = new ReleaseEventForApiContract(_releaseEvent, ContentLanguagePreference.English, ReleaseEventOptionalFields.None, thumbPersister: null);

		await _queries.UpdateBasicProperties(contract);

		_song.ReleaseEvent.Should().BeSameAs(_releaseEvent, "ReleaseEvent");

		_queries.Merge(_song.Id, song2.Id);

		song2.ReleaseEvent.Should().BeSameAs(_releaseEvent, "ReleaseEvent");
	}

	[TestMethod]
	public async Task Merge_WithReleaseEvent()
	{
		var song2 = CreateEntry.Song();
		_repository.Save(song2);

		var relEvent = _repository.Save(CreateEntry.ReleaseEvent("Miku's birthday", new DateTime(2007, 8, 31)));
		song2.ReleaseEvent = relEvent;

		var contract = EditContract();
		contract.ReleaseEvent = new ReleaseEventForApiContract(_releaseEvent, ContentLanguagePreference.English, ReleaseEventOptionalFields.None, thumbPersister: null);

		await _queries.UpdateBasicProperties(contract);

		_song.ReleaseEvent.Should().BeSameAs(_releaseEvent, "ReleaseEvent");

		_queries.Merge(_song.Id, song2.Id);

		song2.ReleaseEvent.Should().BeSameAs(relEvent, "ReleaseEvent");
	}
}
