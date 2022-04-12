using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue1062
{
	[TestClass]
	public class SongQueriesTests
	{
		private Song _song = default!;
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
			_song = CreateEntry.Song(id: 1, name: "Nebula");
			_repository = new FakeSongRepository(_song);
			_repository.SaveNames(_song);

			_user = CreateEntry.User(name: "Miku", group: UserGroupId.Moderator);
			_repository.Save(_user);

			_permissionContext = new FakePermissionContext(_user);
			_entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

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

		private Task<(bool created, int reportId)> CallCreateReport(
			SongReportType reportType,
			string notes = "It's Miku, not Rin",
			int? versionNumber = null
		)
		{
			return _queries.CreateReport(_song.Id, reportType, "39.39.39.39", notes, versionNumber);
		}

		[TestMethod]
		public async Task CreateReport_BrokenPV_WithNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.BrokenPV);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_BrokenPV_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.BrokenPV, notes: string.Empty);

			created.Should().BeFalse("Report was not created");
		}

		[TestMethod]
		public async Task CreateReport_InvalidInfo_WithNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.InvalidInfo);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_InvalidInfo_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.InvalidInfo, notes: string.Empty);

			created.Should().BeFalse("Report was not created");
		}

		[TestMethod]
		public async Task CreateReport_Duplicate_WithNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.Duplicate);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Duplicate_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.Duplicate, notes: string.Empty);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Inappropriate_WithNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.Inappropriate);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Inappropriate_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.Inappropriate, notes: string.Empty);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Other_WithNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.Other);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Other_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(SongReportType.Other, notes: string.Empty);

			created.Should().BeFalse("Report was not created");
		}
	}
}
