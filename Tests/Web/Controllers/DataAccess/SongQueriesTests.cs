#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources.Messages;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="SongQueries"/>.
	/// </summary>
	[TestClass]
	public class SongQueriesTests
	{
		private const int ShortVersionTagId = 4717;
		private EntryAnchorFactory _entryLinkFactory;
		private FakeUserMessageMailer _mailer;
		private CreateSongContract _newSongContract;
		private FakePermissionContext _permissionContext;
		private Artist _producer;
		private FakePVParser _pvParser;
		private FakeSongRepository _repository;
		private SongQueries _queries;
		private ReleaseEvent _releaseEvent;
		private Song _song;
		private Tag _tag;
		private User _user;
		private User _user2;
		private User _user3;
		private Artist _vocalist;
		private Artist _vocalist2;

		private Task<SongContract> CallCreate()
		{
			return _queries.Create(_newSongContract);
		}

		private Task<NewSongCheckResultContract> CallFindDuplicates(string[] anyName = null, string[] anyPv = null, int[] artistIds = null, bool getPvInfo = true)
		{
			return _queries.FindDuplicates(anyName ?? new string[0], anyPv ?? new string[0], artistIds ?? new int[0], getPvInfo);
		}

		private (bool created, SongReport report) CallCreateReport(SongReportType reportType, int? versionNumber = null, Song song = null, DateTime? created = null)
		{
			song ??= _song;
			var result = _queries.CreateReport(song.Id, reportType, "39.39.39.39", "It's Miku, not Rin", versionNumber);
			var report = _repository.Load<SongReport>(result.reportId);
			if (created != null)
				report.Created = created.Value;
			return (result.created, report);
		}

		private SongForEditContract EditContract()
		{
			return new SongForEditContract(_song, ContentLanguagePreference.English);
		}

		private void AssertHasArtist(Song song, Artist artist, ArtistRoles? roles = null)
		{
			song.Artists.Any(a => a.Artist.Equals(artist)).Should().BeTrue(song + " has " + artist);
			if (roles.HasValue)
				song.Artists.Any(a => a.Artist.Equals(artist) && a.Roles == roles).Should().BeTrue(artist + " has roles " + roles);
		}

		private ArtistForSongContract CreateArtistForSongContract(int artistId = 0, string artistName = null, ArtistRoles roles = ArtistRoles.Default)
		{
			if (artistId != 0)
				return new ArtistForSongContract { Artist = new ArtistContract { Name = artistName, Id = artistId }, Roles = roles };
			else
				return new ArtistForSongContract { Name = artistName, Roles = roles };
		}

		private T Save<T>(T entry) where T : class, IDatabaseObject
		{
			return _repository.Save(entry);
		}

		[TestInitialize]
		public void SetUp()
		{
			_producer = CreateEntry.Producer(id: 1, name: "Tripshots");
			_vocalist = CreateEntry.Vocalist(id: 39, name: "Hatsune Miku");
			_vocalist2 = CreateEntry.Vocalist(id: 40, name: "Kagamine Rin");

			_song = CreateEntry.Song(id: 1, name: "Nebula");
			_song.LengthSeconds = 39;
			_repository = new FakeSongRepository(_song);
			Save(_song.AddArtist(_producer));
			Save(_song.AddArtist(_vocalist));
			Save(_song.CreatePV(new PVContract { Id = 1, Service = PVService.Youtube, PVId = "hoLu7c2XZYU", Name = "Nebula", PVType = PVType.Original }));
			_repository.SaveNames(_song);

			_user = CreateEntry.User(id: 1, name: "Miku");
			_user.GroupId = UserGroupId.Trusted;
			_user2 = CreateEntry.User(id: 2, name: "Rin", email: "rin@vocadb.net");
			_user3 = CreateEntry.User(id: 3, name: "Luka", email: "luka@vocadb.net");
			_repository.Add(_user, _user2);
			_repository.Add(_producer, _vocalist);

			_tag = new Tag("vocarock");
			_repository.Save(_tag, new Tag("vocaloud"));

			_releaseEvent = _repository.Save(CreateEntry.ReleaseEvent("Comiket 39"));

			_permissionContext = new FakePermissionContext(_user);
			_entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			_newSongContract = new CreateSongContract
			{
				SongType = SongType.Original,
				Names = new[] {
					new LocalizedStringContract("Resistance", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistForSongContract { Artist = new ArtistContract(_producer, ContentLanguagePreference.Default) },
					new ArtistForSongContract { Artist = new ArtistContract(_vocalist, ContentLanguagePreference.Default) },
				},
				PVUrls = new[] { "http://test.vocadb.net/" }
			};

			_pvParser = new FakePVParser();
			_pvParser.ResultFunc = (url, getMeta) =>
				VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939",
				getMeta ? VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", null, "testimg.jpg", 39) : VideoTitleParseResult.Empty);

			_mailer = new FakeUserMessageMailer();

			_queries = new SongQueries(_repository, _permissionContext, _entryLinkFactory, _pvParser, _mailer,
				new FakeLanguageDetector(), new FakeUserIconFactory(), new EnumTranslations(), new InMemoryImagePersister(), new FakeObjectCache(), new Model.Utils.Config.VdbConfigManager(), new EntrySubTypeNameFactory(),
				new FollowedArtistNotifier(new FakeEntryLinkFactory(), _mailer, new EnumTranslations(), new EntrySubTypeNameFactory()));
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await CallCreate();

			result.Should().NotBeNull("result");
			result.Name.Should().Be("Resistance", "Name");

			_song = _repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			_song.Should().NotBeNull("Song was saved to repository");
			_song.DefaultName.Should().Be("Resistance", "Name");
			_song.Names.SortNames.DefaultLanguage.Should().Be(ContentLanguageSelection.English, "Default language should be English");
			_song.AllArtists.Count.Should().Be(2, "Artists count");
			VocaDbAssert.ContainsArtists(_song.AllArtists, "Tripshots", "Hatsune Miku");
			_song.ArtistString.Default.Should().Be("Tripshots feat. Hatsune Miku", "ArtistString");
			_song.LengthSeconds.Should().Be(39, "Length");  // From PV
			_song.PVServices.Should().Be(PVServices.NicoNicoDouga, "PVServices");

			var archivedVersion = _repository.List<ArchivedSongVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Song.Should().Be(_song, "Archived version song");
			archivedVersion.Reason.Should().Be(SongArchiveReason.Created, "Archived version reason");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			activityEntry.Should().NotBeNull("Activity entry was created");
			activityEntry.EntryBase.Should().Be(_song, "Activity entry's entry");
			activityEntry.EditEvent.Should().Be(EntryEditEvent.Created, "Activity entry event type");

			var pv = _repository.List<PVForSong>().FirstOrDefault(p => p.Song.Id == _song.Id);

			pv.Should().NotBeNull("PV was created");
			pv.Song.Should().Be(_song, "pv.Song");
			pv.Name.Should().Be("Resistance", "pv.Name");
		}

		[TestMethod]
		public void Create_Notification()
		{
			_repository.Save(_user2.AddArtist(_producer));

			CallCreate();

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			notification.Should().NotBeNull("Notification was created");
			notification.Receiver.Should().Be(_user2, "Receiver");
		}

		[TestMethod]
		public void Create_NoNotificationForSelf()
		{
			_repository.Save(_user.AddArtist(_producer));

			CallCreate();

			_repository.List<UserMessage>().Any().Should().BeFalse("No notification was created");
		}

		[TestMethod]
		public void Create_EmailNotification()
		{
			var subscription = _repository.Save(_user2.AddArtist(_producer));
			subscription.EmailNotifications = true;

			CallCreate();

			var notification = _repository.List<UserMessage>().First();

			_mailer.Subject.Should().Be(notification.Subject, "Subject");
			_mailer.Body.Should().NotBeNull("Body");
			_mailer.ReceiverName.Should().Be(notification.Receiver.Name, "ReceiverName");
		}

		[TestMethod]
		public void Create_NotificationForTags()
		{
			_repository.Save(_user2.AddTag(_tag));
			_repository.Save(new TagMapping(_tag, "VOCAROCK"));
			_pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "VOCAROCK" });

			CallCreate();

			var notification = _repository.List<UserMessage>().FirstOrDefault();
			notification.Should().NotBeNull("Notification was created");
			notification.User.Should().Be(_user2, "Notified user");
		}

		[TestMethod]
		public void Create_NoPermission()
		{
			_user.GroupId = UserGroupId.Limited;
			_permissionContext.RefreshLoggedUser(_repository);

			this.Invoking(subject => subject.CallCreate()).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void Create_Tags()
		{
			_repository.Save(new TagMapping(_tag, "VOCAROCK"));
			_pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "VOCAROCK" });

			CallCreate();

			_song = _repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			_song.Tags.Tags.Count().Should().Be(1, "Tags.Count");
			_song.Tags.HasTag(_tag).Should().BeTrue("Has vocarock tag");
		}

		[TestMethod]
		public void Create_Tags_IgnoreDuplicates()
		{
			_repository.Save(_user2.AddTag(_tag));
			_repository.Save(new TagMapping(_tag, "VOCAROCK"));
			_repository.Save(new TagMapping(_tag, "rock"));
			_pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "VOCAROCK", "rock" });

			CallCreate();

			_song = _repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			_song.Tags.Tags.Count().Should().Be(1, "Tags.Count");
			_song.Tags.HasTag(_tag).Should().BeTrue("Has vocarock tag");
			_song.Tags.GetTagUsage(_tag).Count.Should().Be(1, "Tag vote count");
			var messages = _repository.List<UserMessage>().Where(u => u.User.Equals(_user2)).ToArray();
			messages.Length.Should().Be(1, "Notification was sent");
			var message = messages[0];
			message.Receiver.Should().Be(_user2, "Message receiver");
			message.Subject.Should().Be("New song tagged with vocarock", "Message subject"); // Test subject to make sure it's for one tag
		}

		[TestMethod]
		public async Task Create_Tags_IgnoreCoverIfSongTypeIsCover()
		{
			var coverTag = _repository.Save(CreateEntry.Tag("cover"));
			_repository.Save(new TagMapping(coverTag, "cover"));
			_repository.Save(new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Song, SongType.Cover), coverTag));

			_pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "cover" });
			_newSongContract.SongType = SongType.Cover;

			var id = (await CallCreate()).Id;

			_song = _repository.Load(id);

			_song.SongType.Should().Be(SongType.Cover, "SongType");
			_song.Tags.Tags.Count().Should().Be(0, "No tags added");
		}

		[TestMethod]
		public async Task Create_NoPV()
		{
			_newSongContract.PVUrls = new string[0];

			var result = await CallCreate();

			result.Should().NotBeNull("result");
			result.PVServices.Should().Be(PVServices.Nothing, "PVServices");
		}

		[TestMethod]
		public async Task Create_EmptyPV()
		{
			_newSongContract.PVUrls = new[] { string.Empty };

			var result = await CallCreate();

			result.Should().NotBeNull("result");
			result.PVServices.Should().Be(PVServices.Nothing, "PVServices");
		}

		/// <summary>
		/// Basic report, no existing reports.
		/// </summary>
		[TestMethod]
		public void CreateReport()
		{
			var (created, report) = CallCreateReport(SongReportType.InvalidInfo);

			created.Should().BeTrue("Report was created");
			report.EntryBase.Id.Should().Be(_song.Id, "Entry Id");
			report.User.Should().Be(_user, "Report author");
			report.ReportType.Should().Be(SongReportType.InvalidInfo, "Report type");
		}

		/// <summary>
		/// Report specific version.
		/// </summary>
		[TestMethod]
		public void CreateReport_Version()
		{
			var version = ArchivedSongVersion.Create(_song, new SongDiff(), new AgentLoginData(_user), SongArchiveReason.PropertiesUpdated, String.Empty);
			_repository.Save(version);
			CallCreateReport(SongReportType.Other, version.Version);

			var report = _repository.List<SongReport>().First();

			report.VersionNumber.Should().Be(version.Version, "Version number");
			report.VersionBase.Should().NotBeNull("VersionBase");

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			notification.Should().NotBeNull("Notification was created");
			notification.Receiver.Should().Be(_user, "Notification receiver");
			notification.Subject.Should().Be(string.Format(EntryReportStrings.EntryVersionReportTitle, _song.DefaultName), "Notification subject");
			notification.Message.Should().Be(string.Format(EntryReportStrings.EntryVersionReportBody, MarkdownHelper.CreateMarkdownLink(_entryLinkFactory.GetFullEntryUrl(_song), _song.DefaultName), "It's Miku, not Rin"), "Notification message");
		}

		/// <summary>
		/// Open report exists for another entry.
		/// </summary>
		[TestMethod]
		public void CreateReport_OtherEntry()
		{
			var song2 = _repository.Save(CreateEntry.Song());
			CallCreateReport(SongReportType.InvalidInfo, song: song2);
			var secondResult = CallCreateReport(SongReportType.Other);

			var reports = _repository.List<SongReport>();

			reports.Count.Should().Be(2, "Number of reports");
			secondResult.created.Should().BeTrue("Second report was created");
		}

		/// <summary>
		/// Duplicate report exists: skip.
		/// </summary>
		[TestMethod]
		public void CreateReport_Duplicate()
		{
			var (_, report) = CallCreateReport(SongReportType.InvalidInfo);
			var secondResult = CallCreateReport(SongReportType.Other);

			var reports = _repository.List<SongReport>();

			reports.Count.Should().Be(1, "Number of reports");
			report.ReportType.Should().Be(SongReportType.InvalidInfo, "Report type");
			secondResult.created.Should().BeFalse("Second report was not created");
		}

		/// <summary>
		/// Duplicate report exists, but it closed.
		/// </summary>
		[TestMethod]
		public void CreateReport_Duplicate_Closed()
		{
			var (_, report) = CallCreateReport(SongReportType.InvalidInfo);
			report.Status = ReportStatus.Closed;
			var (secondCreated, _) = CallCreateReport(SongReportType.Other);

			var reports = _repository.List<SongReport>();

			reports.Count.Should().Be(2, "Number of reports");
			secondCreated.Should().BeTrue("Second report was created");
		}

		/// <summary>
		/// Duplicate report exists. It is closed, but current user is not logged in. Skip.
		/// </summary>
		[TestMethod]
		public void CreateReport_Duplicate_Closed_NotLoggedIn()
		{
			_permissionContext.LoggedUser = null;
			var (_, report) = CallCreateReport(SongReportType.InvalidInfo);
			report.Status = ReportStatus.Closed;
			var (secondCreated, _) = CallCreateReport(SongReportType.Other);

			var reports = _repository.List<SongReport>();

			reports.Count.Should().Be(1, "Number of reports");
			secondCreated.Should().BeFalse("Second report was not created");
		}

		/// <summary>
		/// Duplicate reports exist. One is closed, the other one open.
		/// Skip creating third report because of open report.
		/// </summary>
		[TestMethod]
		public void CreateReport_Duplicate_Closed_Then_Open()
		{
			var (_, report) = CallCreateReport(SongReportType.InvalidInfo, created: DateTime.UtcNow.AddDays(-2));
			report.Status = ReportStatus.Closed;
			CallCreateReport(SongReportType.Other, created: DateTime.UtcNow.AddDays(-1));
			var (thirdCreated, _) = CallCreateReport(SongReportType.Other);

			var reports = _repository.List<SongReport>();

			reports.Should().HaveCount(2);
			thirdCreated.Should().BeFalse("Third report was not created");
		}

		/// <summary>
		/// Create report when not logged in.
		/// Report is created using hostname.
		/// </summary>
		[TestMethod]
		public void CreateReport_NotLoggedIn()
		{
			_permissionContext.LoggedUser = null;
			CallCreateReport(SongReportType.Other);

			var report = _repository.List<SongReport>().FirstOrDefault();

			report.Should().NotBeNull("Report was created");
			report.User.Should().BeNull("User is null");
			report.Hostname.Should().Be("39.39.39.39", "Hostname");
		}

		// Create report, notify the user who created the entry if they're the only editor.
		[TestMethod]
		public void CreateReport_NotifyIfUnambiguous()
		{
			var editor = _user2;
			_repository.Save(ArchivedSongVersion.Create(_song, new SongDiff(), new AgentLoginData(editor), SongArchiveReason.PropertiesUpdated, String.Empty));
			_queries.CreateReport(_song.Id, SongReportType.Other, "39.39.39.39", "It's Miku, not Rin", null);

			var report = _repository.List<SongReport>().First();
			report.Entry.Should().Be(_song, "Report was created for song");
			report.Version.Should().BeNull("Version");

			var notification = _repository.List<UserMessage>().FirstOrDefault();
			notification.Should().NotBeNull("notification was created");
			notification.Receiver.Should().Be(editor, "notification was receiver is editor");
			notification.Subject.Should().Be(string.Format(EntryReportStrings.EntryVersionReportTitle, _song.DefaultName), "Notification subject");
		}

		[TestMethod]
		public void CreateReport_DoNotNotifyIfAmbiguous()
		{
			_repository.Save(ArchivedSongVersion.Create(_song, new SongDiff(), new AgentLoginData(_user2), SongArchiveReason.PropertiesUpdated, String.Empty));
			_repository.Save(ArchivedSongVersion.Create(_song, new SongDiff(), new AgentLoginData(_user3), SongArchiveReason.PropertiesUpdated, String.Empty));
			_queries.CreateReport(_song.Id, SongReportType.Other, "39.39.39.39", "It's Miku, not Rin", null);

			var report = _repository.List<SongReport>().First();
			report.Entry.Should().Be(_song, "Report was created for song");
			report.Version.Should().BeNull("Version");

			var notification = _repository.List<UserMessage>().FirstOrDefault();
			notification.Should().BeNull("notification was not created");
		}

		// Create report, with both report type name and notes
		[TestMethod]
		public void CreateReport_Notify_ReportTypeNameAndNotes()
		{
			var editor = _user2;
			_repository.Save(ArchivedSongVersion.Create(_song, new SongDiff(), new AgentLoginData(editor), SongArchiveReason.PropertiesUpdated, String.Empty));
			_queries.CreateReport(_song.Id, SongReportType.BrokenPV, "39.39.39.39", "It's Miku, not Rin", null);

			var entryLink = MarkdownHelper.CreateMarkdownLink(_entryLinkFactory.GetFullEntryUrl(_song), _song.DefaultName);

			var notification = _repository.List<UserMessage>().FirstOrDefault();
			notification.Should().NotBeNull("notification was created");
			notification.Subject.Should().Be(string.Format(EntryReportStrings.EntryVersionReportTitle, _song.DefaultName), "Notification subject");
			notification.Message.Should().Be(string.Format(EntryReportStrings.EntryVersionReportBody, entryLink, "Broken PV (It's Miku, not Rin)"), "Notification body");
		}


		// Two PVs, no matches, parse song info from the NND PV.
		[TestMethod]
		public async Task FindDuplicates_NoMatches_ParsePVInfo()
		{
			// Note: Nico will be preferred, if available
			_pvParser.MatchedPVs.Add("http://youtu.be/123456567",
				VideoUrlParseResult.CreateOk("http://youtu.be/123456567", PVService.Youtube, "123456567",
				VideoTitleParseResult.CreateSuccess("anger PV", "Tripshots", null, "testimg2.jpg", 33)));

			_pvParser.MatchedPVs.Add("http://www.nicovideo.jp/watch/sm3183550",
				VideoUrlParseResult.CreateOk("http://www.nicovideo.jp/watch/sm3183550", PVService.NicoNicoDouga, "sm3183550",
				VideoTitleParseResult.CreateSuccess("【初音ミク】anger【VOCALOID3DPV】", "Tripshots", null, "testimg.jpg", 39)));

			var result = await CallFindDuplicates(new[] { "【初音ミク】anger【VOCALOID3DPV】" }, new[] { "http://youtu.be/123456567", "http://www.nicovideo.jp/watch/sm3183550" });

			result.Title.Should().Be("anger", "Title"); // Title from PV
			result.Matches.Length.Should().Be(0, "No matches");
		}

		[TestMethod]
		public async Task FindDuplicates_ParsePVInfo_YouTube()
		{
			var artist = _repository.Save(CreateEntry.Artist(ArtistType.Producer, name: "Clean Tears"));
			_repository.Save(artist.CreateWebLink("YouTube", "https://www.youtube.com/channel/UCnHGCQ0pwnRFF5Oe2YTeOcA", WebLinkCategory.Official, disabled: false));

			var titleParseResult = VideoTitleParseResult.CreateSuccess("Clean Tears - Ruby", "Clean Tears", null, "http://tn.smilevideo.jp/smile?i=32347786", 39);
			titleParseResult.Author = "Clean Tears";
			titleParseResult.AuthorId = "UCnHGCQ0pwnRFF5Oe2YTeOcA";

			_pvParser.MatchedPVs.Add("https://youtu.be/aJKY_EeAeYc",
				VideoUrlParseResult.CreateOk("https://youtu.be/aJKY_EeAeYc", PVService.Youtube, "aJKY_EeAeYc", titleParseResult));

			var result = await CallFindDuplicates(new string[0], new[] { "https://youtu.be/aJKY_EeAeYc" });

			result.Title.Should().Be("Clean Tears - Ruby", "Title"); // Title from PV
			result.Artists.Length.Should().Be(1, "Number of matched artists");
			result.Artists[0].Id.Should().Be(artist.Id, "Matched artist");
		}

		[TestMethod]
		public async Task FindDuplicates_MatchName()
		{
			var result = await CallFindDuplicates(new[] { "Nebula" });

			result.Matches.Length.Should().Be(1, "Matches");
			var match = result.Matches.First();
			match.Entry.Id.Should().Be(_song.Id, "Matched song");
			match.MatchProperty.Should().Be(SongMatchProperty.Title, "Matched property");
		}

		[TestMethod]
		public async Task FindDuplicates_MatchNameAndArtist()
		{
			var producer2 = Save(CreateEntry.Artist(ArtistType.Producer, name: "minato"));
			var song2 = _repository.Save(CreateEntry.Song(name: "Nebula"));
			Save(song2.AddArtist(producer2));

			var result = await CallFindDuplicates(new[] { "Nebula" }, artistIds: new[] { producer2.Id });

			// 2 songs, the one with both artist and title match appears first
			result.Matches.Length.Should().Be(2, "Matches");
			var match = result.Matches.First();
			match.Entry.Id.Should().Be(song2.Id, "Matched song");
			match.MatchProperty.Should().Be(SongMatchProperty.Title, "Matched property");
		}

		[TestMethod]
		public async Task FindDuplicates_SkipPVInfo()
		{
			var result = await CallFindDuplicates(new[] { "Anger" }, new[] { "http://www.nicovideo.jp/watch/sm393939" }, getPvInfo: false);

			result.Title.Should().BeNull("Title");
			result.Matches.Length.Should().Be(0, "No matches");
		}

		[TestMethod]
		public async Task FindDuplicates_MatchPV()
		{
			_pvParser.MatchedPVs.Add("http://youtu.be/hoLu7c2XZYU",
				VideoUrlParseResult.CreateOk("http://youtu.be/hoLu7c2XZYU", PVService.Youtube, "hoLu7c2XZYU", VideoTitleParseResult.Empty));

			var result = await CallFindDuplicates(anyPv: new[] { "http://youtu.be/hoLu7c2XZYU" });

			result.Matches.Length.Should().Be(1, "Matches");
			var match = result.Matches.First();
			match.Entry.Id.Should().Be(_song.Id, "Matched song");
			match.MatchProperty.Should().Be(SongMatchProperty.PV, "Matched property");
		}

		[TestMethod]
		public async Task FindDuplicates_CoverInSongTitle_CoverType()
		{
			_pvParser.MatchedPVs.Add("http://www.nicovideo.jp/watch/sm27114783",
				VideoUrlParseResult.CreateOk("http://www.nicovideo.jp/watch/sm27114783", PVService.NicoNicoDouga, "123456567",
				VideoTitleParseResult.CreateSuccess("【GUMI】 光(宇多田ヒカル) 【アレンジカバー】", string.Empty, null, "testimg2.jpg", 33)));

			var result = await CallFindDuplicates(anyPv: new[] { "http://www.nicovideo.jp/watch/sm27114783" });

			result.SongType.Should().Be(SongType.Cover, "SongType is cover because of the 'cover' in title");
		}

		[TestMethod]
		public void GetRelatedSongs()
		{
			var matchingArtist = Save(CreateEntry.Song());
			Save(matchingArtist.AddArtist(_song.Artists.First().Artist));

			Save(_song.AddTag(_tag).Result);
			var matchingTag = Save(CreateEntry.Song());
			Save(matchingTag.AddTag(_tag).Result);

			Save(_user.AddSongToFavorites(_song, SongVoteRating.Like));
			var matchingLike = Save(CreateEntry.Song());
			Save(_user.AddSongToFavorites(matchingLike, SongVoteRating.Like));

			// Unrelated song
			Save(CreateEntry.Song());

			var result = _queries.GetRelatedSongs(_song.Id, SongOptionalFields.AdditionalNames);

			result.Should().NotBeNull("result");
			result.ArtistMatches.Length.Should().Be(1, "Number of artist matches");
			result.ArtistMatches.First().Id.Should().Be(matchingArtist.Id, "Matching artist");
			result.TagMatches.Length.Should().Be(1, "Number of tag matches");
			result.TagMatches.First().Id.Should().Be(matchingTag.Id, "Matching tag");
			result.LikeMatches.Length.Should().Be(1, "Number of like matches");
			result.LikeMatches.First().Id.Should().Be(matchingLike.Id, "Matching like");
		}

		[TestMethod]
		public void GetSongForEdit()
		{
			var album = _repository.Save(CreateEntry.Album());
			album.OriginalRelease.ReleaseDate = new OptionalDateTime(2007, 8, 31);
			var relEvent = _repository.Save(CreateEntry.ReleaseEvent("Miku's birthday", new DateTime(2007, 8, 31)));
			album.OriginalRelease.ReleaseEvent = relEvent;
			album.AddSong(_song, 1, 1);

			var album2 = _repository.Save(CreateEntry.Album());
			album2.OriginalRelease.ReleaseDate = new OptionalDateTime(2017, 8, 31);
			album2.AddSong(_song, 1, 2);

			var result = _queries.GetSongForEdit(_song.Id);

			result.Should().NotBeNull("result");
			result.AlbumEventId.Should().Be(relEvent.Id, "AlbumEventId");
		}

		[TestMethod]
		public async Task GetTagSuggestions()
		{
			var tag2 = _repository.Save(CreateEntry.Tag("metalcore"));
			_repository.Save(new TagMapping(_tag, "vocarock"));
			_repository.Save(new TagMapping(tag2, "vocacore"));

			_pvParser.ResultFunc = (url, getMeta) =>
				VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939",
					getMeta ? VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", null, "testimg.jpg", tags: new[] { "vocarock", "vocacore" }) : VideoTitleParseResult.Empty);

			_song.AddTag(_tag);
			_song.PVs.Add(new PVForSong(_song, new PVContract { Service = PVService.NicoNicoDouga, PVType = PVType.Original, PVId = "sm393939" }));

			var result = await _queries.GetTagSuggestionsAsync(_song.Id);

			result.Count.Should().Be(1, "One suggestion");
			result[0].Tag.Name.Should().Be("metalcore", "Tag name");
		}

		[TestMethod]
		public async Task GetTagSuggestions_IgnoreCoverTagIfTypeIsCover()
		{
			var coverTag = _repository.Save(CreateEntry.Tag("cover"));
			_repository.Save(new TagMapping(coverTag, "cover"));
			_repository.Save(new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Song, SongType.Cover), coverTag));

			_pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "cover" });
			_song.SongType = SongType.Cover;
			_song.PVs.Add(new PVForSong(_song, new PVContract { Service = PVService.NicoNicoDouga, PVType = PVType.Original, PVId = "sm393939" }));

			var result = await _queries.GetTagSuggestionsAsync(_song.Id);

			result.Count.Should().Be(0, "Cover tag suggestion ignored");
		}

		[TestMethod]
		public async Task GetTagSuggestions_ShortVersion()
		{
			var shortVersionTag = _repository.Save(CreateEntry.Tag("short version", ShortVersionTagId));

			_song.LengthSeconds = 3939;

			var remix = _repository.Save(CreateEntry.Song());
			remix.SongType = SongType.Remix;
			remix.LengthSeconds = 39;
			remix.SetOriginalVersion(_song);

			var result = await _queries.GetTagSuggestionsAsync(remix.Id);

			result.FirstOrDefault()?.Tag.Name.Should().Be("short version", "Short version tag was returned");
		}

		[TestMethod]
		public void Merge_ToEmpty()
		{
			_song.Notes.Original = "Notes";
			var song2 = new Song();
			_repository.Save(song2);

			_queries.Merge(_song.Id, song2.Id);

			song2.Names.AllValues.FirstOrDefault().Should().Be("Nebula", "Name");
			song2.AllArtists.Count.Should().Be(2, "Artists");
			AssertHasArtist(song2, _producer);
			AssertHasArtist(song2, _vocalist);
			song2.LengthSeconds.Should().Be(_song.LengthSeconds, "LengthSeconds");
			song2.Notes.Original.Should().Be(_song.Notes.Original, "Notes were copied");

			var mergeRecord = _repository.List<SongMergeRecord>().FirstOrDefault();
			mergeRecord.Should().NotBeNull("Merge record was created");
			mergeRecord.Source.Should().Be(_song.Id, "mergeRecord.Source");
			mergeRecord.Target.Id.Should().Be(song2.Id, "mergeRecord.Target.Id");
		}

		[TestMethod]
		public void Merge_WithArtists()
		{
			_song.GetArtistLink(_producer).Roles = ArtistRoles.Instrumentalist;
			var song2 = CreateEntry.Song();
			_repository.Save(song2);
			song2.AddArtist(_vocalist);
			song2.AddArtist(_vocalist2).Roles = ArtistRoles.Other;

			_queries.Merge(_song.Id, song2.Id);
			song2.AllArtists.Count.Should().Be(3, "Artists");
			AssertHasArtist(song2, _producer, ArtistRoles.Instrumentalist);
			AssertHasArtist(song2, _vocalist2, ArtistRoles.Other);
		}

		[TestMethod]
		public void Merge_NoPermissions()
		{
			_user.GroupId = UserGroupId.Regular;
			_permissionContext.RefreshLoggedUser(_repository);

			var song2 = new Song();
			_repository.Save(song2);

			_queries.Invoking(subject => subject.Merge(_song.Id, song2.Id)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public async Task Revert()
		{
			_user.GroupId = UserGroupId.Moderator;
			_permissionContext.RefreshLoggedUser(_repository);
			SongForEditContract Contract()
			{
				return new SongForEditContract(_song, ContentLanguagePreference.English);
			}

			await _queries.UpdateBasicProperties(Contract());

			// Remove all artists
			var contract = Contract();
			contract.Artists = new ArtistForSongContract[0];
			await _queries.UpdateBasicProperties(contract);

			var latestVersionBeforeRevert = _song.ArchivedVersionsManager.GetLatestVersion();
			latestVersionBeforeRevert.Should().NotBeNull("latestVersion");
			_song.Version.Should().Be(2, "Version number before revert");

			_queries.RevertToVersion(latestVersionBeforeRevert.Id);

			_song.Version.Should().Be(3, "Version was incremented");
			_song.Artists.Count().Should().Be(2, "Artist links were restored");
			string.Empty.Should().NotBe(_song.ArtistString?.Default, "Artist string was restored");

			var latestVersion = _song.ArchivedVersionsManager.GetLatestVersion();
			latestVersion.Reason.Should().Be(SongArchiveReason.Reverted, "Reason");
			latestVersion.IsIncluded(SongEditableFields.Artists).Should().BeTrue("Artists are included in diff");
		}

		[TestMethod]
		public async Task Update_Names()
		{
			var contract = new SongForEditContract(_song, ContentLanguagePreference.English);
			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated song";

			contract = await _queries.UpdateBasicProperties(contract);

			var songFromRepo = _repository.Load(contract.Id);
			songFromRepo.DefaultName.Should().Be("Replaced name");
			songFromRepo.Version.Should().Be(1, "Version");
			songFromRepo.AllArtists.Count.Should().Be(2, "Number of artists");
			songFromRepo.AllAlbums.Count.Should().Be(0, "No albums");

			var archivedVersion = _repository.List<ArchivedSongVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Song.Should().Be(_song, "Archived version song");
			archivedVersion.Reason.Should().Be(SongArchiveReason.PropertiesUpdated, "Archived version reason");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(SongEditableFields.Names, "Changed fields");

			var activityEntry = _repository.List<ActivityEntry>().FirstOrDefault();

			activityEntry.Should().NotBeNull("Activity entry was created");
			activityEntry.EntryBase.Should().Be(_song, "Activity entry's entry");
			activityEntry.EditEvent.Should().Be(EntryEditEvent.Updated, "Activity entry event type");
		}

		[TestMethod]
		public async Task Update_Artists()
		{
			var newSong = CreateEntry.Song(name: "Anger");

			_repository.Save(newSong);

			foreach (var name in newSong.Names)
				_repository.Save(name);

			var contract = new SongForEditContract(newSong, ContentLanguagePreference.English);
			contract.Artists = new[] {
				CreateArtistForSongContract(artistId: _producer.Id),
				CreateArtistForSongContract(artistId: _vocalist.Id),
				CreateArtistForSongContract(artistName: "Goomeh", roles: ArtistRoles.Vocalist),
			};

			contract = await _queries.UpdateBasicProperties(contract);

			var songFromRepo = _repository.Load(contract.Id);

			songFromRepo.AllArtists.Count.Should().Be(3, "Number of artists");

			AssertHasArtist(songFromRepo, _producer);
			AssertHasArtist(songFromRepo, _vocalist);
			songFromRepo.ArtistString.Default.Should().Be("Tripshots feat. Hatsune Miku, Goomeh", "Artist string");

			var archivedVersion = _repository.List<ArchivedSongVersion>().FirstOrDefault();

			archivedVersion.Should().NotBeNull("Archived version was created");
			archivedVersion.Diff.ChangedFields.Value.Should().Be(SongEditableFields.Artists, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Artists_Notify()
		{
			_repository.Save(_user2.AddArtist(_vocalist2));
			_repository.Save(_vocalist2);

			var contract = new SongForEditContract(_song, ContentLanguagePreference.English);
			contract.Artists = contract.Artists.Concat(new[] { CreateArtistForSongContract(_vocalist2.Id) }).ToArray();

			await _queries.UpdateBasicProperties(contract);

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			notification.Should().NotBeNull("Notification was created");
			notification.Receiver.Should().Be(_user2, "Receiver");
		}

		[TestMethod]
		public async Task Update_Artists_RemoveDeleted()
		{
			_repository.Save(_vocalist2);
			_repository.Save(_song.AddArtist(_vocalist2));
			_vocalist2.Deleted = true;

			var contract = new SongForEditContract(_song, ContentLanguagePreference.English);

			await _queries.UpdateBasicProperties(contract);

			_song.AllArtists.Any(a => Equals(_vocalist2, a.Artist)).Should().BeFalse("vocalist2 was removed from song");
		}

		[TestMethod]
		public async Task Update_Lyrics()
		{
			var contract = EditContract();
			contract.Lyrics = new[] {
				CreateEntry.LyricsForSongContract(cultureCode: OptionalCultureCode.LanguageCode_English, translationType: TranslationType.Original)
			};

			await _queries.UpdateBasicProperties(contract);

			_song.Lyrics.Count.Should().Be(1, "Lyrics were added");
			var lyrics = _song.Lyrics.First();
			lyrics.Value.Should().Be("Miku Miku", "Lyrics text");
		}

		[TestMethod]
		public async Task Update_PublishDate_From_PVs()
		{
			var contract = new SongForEditContract(_song, ContentLanguagePreference.English);
			contract.PVs = new[] {
				CreateEntry.PVContract(id: 1, pvId: "hoLu7c2XZYU", pvType: PVType.Reprint, publishDate: new DateTime(2015, 3, 9, 10, 0, 0)),
				CreateEntry.PVContract(id: 2, pvId: "mikumikumiku", pvType: PVType.Original, publishDate: new DateTime(2015, 4, 9, 16, 0, 0))
			};

			contract = await _queries.UpdateBasicProperties(contract);

			var songFromRepo = _repository.Load(contract.Id);
			songFromRepo.PVs.PVs.Count.Should().Be(2, "Number of PVs");
			songFromRepo.PublishDate.DateTime.Should().Be(new DateTime(2015, 4, 9), "Song publish date was updated");
		}

		[TestMethod]
		public async Task Update_Weblinks()
		{
			var contract = new SongForEditContract(_song, ContentLanguagePreference.English);
			contract.WebLinks = new[] {
				new WebLinkContract("http://vocadb.net", "VocaDB", WebLinkCategory.Reference, disabled: false)
			};

			contract = await _queries.UpdateBasicProperties(contract);
			var songFromRepo = _repository.Load(contract.Id);
			songFromRepo.WebLinks.Count.Should().Be(1, "Number of weblinks");
			songFromRepo.WebLinks[0].Url.Should().Be("http://vocadb.net", "Weblink URL");
		}

		[TestMethod]
		public async Task Update_Weblinks_SkipWhitespace()
		{
			var contract = new SongForEditContract(_song, ContentLanguagePreference.English);
			contract.WebLinks = new[] {
				new WebLinkContract(" ", "VocaDB", WebLinkCategory.Reference, disabled: false)
			};

			contract = await _queries.UpdateBasicProperties(contract);
			var songFromRepo = _repository.Load(contract.Id);
			songFromRepo.WebLinks.Count.Should().Be(0, "Number of weblinks");
		}

		/// <summary>
		/// User has selected the event
		/// </summary>
		[TestMethod]
		public async Task Update_ReleaseEvent_ExistingEvent_Selected()
		{
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract(_releaseEvent, ContentLanguagePreference.English);

			await _queries.UpdateBasicProperties(contract);

			_song.ReleaseEvent.Should().BeSameAs(_releaseEvent, "ReleaseEvent");
		}

		/// <summary>
		/// User typed an event name, and there's a name match
		/// </summary>
		[TestMethod]
		public async Task Update_ReleaseEvent_ExistingEvent_MatchByName()
		{
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract { Name = _releaseEvent.DefaultName };

			await _queries.UpdateBasicProperties(contract);

			_song.ReleaseEvent.Should().BeSameAs(_releaseEvent, "ReleaseEvent");
		}

		[TestMethod]
		public async Task Update_ReleaseEvent_NewEvent_Standalone()
		{
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract { Name = "Comiket 40" };

			await _queries.UpdateBasicProperties(contract);

			_song.ReleaseEvent.Should().NotBeNull("ReleaseEvent");
			_song.ReleaseEvent.DefaultName.Should().BeSameAs("Comiket 40", "ReleaseEvent.Name");

			_song.ReleaseEvent.ArchivedVersionsManager.Versions.Count.Should().Be(1, "New release event was archived");
		}

		[TestMethod]
		public async Task Update_ReleaseEvent_NewEvent_SeriesEvent()
		{
			var series = _repository.Save(CreateEntry.EventSeries("Comiket"));
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract { Name = "Comiket 40" };

			await _queries.UpdateBasicProperties(contract);

			_song.ReleaseEvent.Should().NotBeNull("ReleaseEvent");
			_song.ReleaseEvent.Series.Should().Be(series, "Series");
			_song.ReleaseEvent.SeriesNumber.Should().Be(40, "SeriesNumber");
		}

		[TestMethod]
		public async Task Update_SendNotificationsForNewPVs()
		{
			_song.PVs.PVs.Clear();
			_song.CreateDate = DateTime.Now - TimeSpan.FromDays(30);
			_repository.Save(_user2.AddArtist(_producer));
			var contract = EditContract();
			contract.PVs = new[] { CreateEntry.PVContract(pvType: PVType.Original) };

			await _queries.UpdateBasicProperties(contract);

			var notifications = _repository.List<UserMessage>();
			notifications.Count.Should().Be(1, "Notification was sent");
			var notification = notifications.First();
			notification.User.Should().Be(_user2, "Notification was sent to user");
		}

		[TestMethod]
		public async Task Update_DoNotSendNotificationsForReprints()
		{
			_song.PVs.PVs.Clear();
			_song.CreateDate = DateTime.Now - TimeSpan.FromDays(30);
			_repository.Save(_user2.AddArtist(_producer));
			var contract = EditContract();
			contract.PVs = new[] { CreateEntry.PVContract(pvType: PVType.Reprint) };

			await _queries.UpdateBasicProperties(contract);

			_repository.Count<UserMessage>().Should().Be(0, "No notification was sent");
		}
	}
}
