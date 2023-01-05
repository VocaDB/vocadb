using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue1109;

[TestClass]
public class SongQueriesTests
{
	private Song _song1;
	private Song _song2;
	private FakePermissionContext _permissionContext = default!;
	private EntryAnchorFactory _entryLinkFactory = default!;
	private FakePVParser _pvParser = default!;
	private SongQueries _queries = default!;
	private FakeSongRepository _repository = default!;
	private FakeUserMessageMailer _mailer = default!;

	/// <summary>
	/// Logged in user
	/// </summary>
	private User _user = default!;

	[TestInitialize]
	public void SetUp()
	{
		_song1 = new Song(TranslatedString.Create("Project Diva desu.")) { Id = 1 };
		_song2 = new Song(TranslatedString.Create("World is Mine")) { Id = 2 };

		_repository = new FakeSongRepository(_song1, _song2);
		_repository.SaveNames(_song1, _song2);

		_user = CreateEntry.User(name: "Miku", group: UserGroupId.Moderator);
		_repository.Save(_user);

		_permissionContext = new FakePermissionContext(_user);
		_entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

		_mailer = new FakeUserMessageMailer();

		_queries = new SongQueries(
			repository: _repository,
			permissionContext: _permissionContext,
			entryLinkFactory: _entryLinkFactory,
			pvParser: _pvParser,
			mailer: _mailer,
			languageDetector: new FakeLanguageDetector(),
			userIconFactory: new FakeUserIconFactory(),
			enumTranslations: new EnumTranslations(),
			entryThumbPersister: new InMemoryImagePersister(),
			cache: new FakeObjectCache(),
			config: new VdbConfigManager(),
			entrySubTypeNameFactory: new EntrySubTypeNameFactory(),
			followedArtistNotifier: new FollowedArtistNotifier(new FakeEntryLinkFactory(), _mailer, new EnumTranslations(), new EntrySubTypeNameFactory()),
			discordWebhookNotifier: new FakeDiscordWebhookNotifier()
		);
	}

	[TestMethod]
	public void GetPublicSongListsForSong()
	{
		var list1 = _repository.Save(new SongList("Mikulist", _user));
		_song1.AllListLinks.Add(_repository.Save(list1.AddSong(_song1)));

		var list2 = _repository.Save(new SongList("Rinlist", _user));
		_song1.AllListLinks.Add(_repository.Save(list2.AddSong(_song1)));
		_song2.AllListLinks.Add(_repository.Save(list2.AddSong(_song2)));

		var list3 = _repository.Save(new SongList("Lukalist", _user) { Deleted = true });
		_song1.AllListLinks.Add(_repository.Save(list3.AddSong(_song1)));
		_song2.AllListLinks.Add(_repository.Save(list3.AddSong(_song2)));

		var contracts1 = _queries.GetPublicSongListsForSong(songId: 1);
		contracts1.Length.Should().Be(2);
		contracts1[0].Name.Should().Be("Mikulist");
		contracts1[1].Name.Should().Be("Rinlist");

		var contracts2 = _queries.GetPublicSongListsForSong(songId: 2);
		contracts2.Length.Should().Be(1);
		contracts2[0].Name.Should().Be("Rinlist");
	}
}
