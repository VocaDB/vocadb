#nullable disable

using VocaDb.Model.Database.Queries;
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers;

/// <summary>
/// Tests for <see cref="SongController"/>.
/// </summary>
[TestClass]
public class SongControllerTests
{
	private readonly SongController _controller;
	private readonly FakePermissionContext _permissionContext = new();
	private readonly FakeSongRepository _repository = new();

	public SongControllerTests()
	{
		_permissionContext.SetLoggedUser(_repository.Save(CreateEntry.User()));
		var queries = new SongQueries(
			_repository,
			_permissionContext,
			new FakeEntryLinkFactory(),
			new FakePVParser(),
			new FakeUserMessageMailer(),
			new FakeLanguageDetector(),
			new FakeUserIconFactory(),
			new EnumTranslations(),
			new InMemoryImagePersister(),
			new FakeObjectCache(),
			new VdbConfigManager(),
			new EntrySubTypeNameFactory(),
			new FakeFollowedArtistNotifier(),
			new FakeDiscordWebhookNotifier());
		_controller = new SongController(
			service: null,
			queries: queries,
			songListQueries: null,
			markdownParser: null,
			pvHelper: null,
			brandableStringsManager: null
		);
	}
}
