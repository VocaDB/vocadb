using System;
using System.Linq;
using System.Threading.Tasks;
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
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;
using FluentAssertions;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="SongQueries"/>.
	/// </summary>
	[TestClass]
	public class SongQueriesTests
	{
		private const int shortVersionTagId = 4717;
		private EntryAnchorFactory entryLinkFactory;
		private FakeUserMessageMailer mailer;
		private CreateSongContract newSongContract;
		private FakePermissionContext permissionContext;
		private Artist producer;
		private FakePVParser pvParser;
		private FakeSongRepository repository;
		private SongQueries queries;
		private ReleaseEvent releaseEvent;
		private Song song;
		private Tag tag;
		private User user;
		private User user2;
		private User user3;
		private Artist vocalist;
		private Artist vocalist2;

		private Task<SongContract> CallCreate()
		{
			return queries.Create(newSongContract);
		}

		private Task<NewSongCheckResultContract> CallFindDuplicates(string[] anyName = null, string[] anyPv = null, int[] artistIds = null, bool getPvInfo = true)
		{
			return queries.FindDuplicates(anyName ?? new string[0], anyPv ?? new string[0], artistIds ?? new int[0], getPvInfo);
		}

		private (bool created, SongReport report) CallCreateReport(SongReportType reportType, int? versionNumber = null, Song song = null, DateTime? created = null)
		{
			song ??= this.song;
			var result = queries.CreateReport(song.Id, reportType, "39.39.39.39", "It's Miku, not Rin", versionNumber);
			var report = repository.Load<SongReport>(result.reportId);
			if (created != null)
				report.Created = created.Value;
			return (result.created, report);
		}

		private SongForEditContract EditContract()
		{
			return new SongForEditContract(song, ContentLanguagePreference.English);
		}

		private void AssertHasArtist(Song song, Artist artist, ArtistRoles? roles = null)
		{
			Assert.IsTrue(song.Artists.Any(a => a.Artist.Equals(artist)), song + " has " + artist);
			if (roles.HasValue)
				Assert.IsTrue(song.Artists.Any(a => a.Artist.Equals(artist) && a.Roles == roles), artist + " has roles " + roles);
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
			return repository.Save(entry);
		}

		[TestInitialize]
		public void SetUp()
		{
			producer = CreateEntry.Producer(id: 1, name: "Tripshots");
			vocalist = CreateEntry.Vocalist(id: 39, name: "Hatsune Miku");
			vocalist2 = CreateEntry.Vocalist(id: 40, name: "Kagamine Rin");

			song = CreateEntry.Song(id: 1, name: "Nebula");
			song.LengthSeconds = 39;
			repository = new FakeSongRepository(song);
			Save(song.AddArtist(producer));
			Save(song.AddArtist(vocalist));
			Save(song.CreatePV(new PVContract { Id = 1, Service = PVService.Youtube, PVId = "hoLu7c2XZYU", Name = "Nebula", PVType = PVType.Original }));
			repository.SaveNames(song);

			user = CreateEntry.User(id: 1, name: "Miku");
			user.GroupId = UserGroupId.Trusted;
			user2 = CreateEntry.User(id: 2, name: "Rin", email: "rin@vocadb.net");
			user3 = CreateEntry.User(id: 3, name: "Luka", email: "luka@vocadb.net");
			repository.Add(user, user2);
			repository.Add(producer, vocalist);

			tag = new Tag("vocarock");
			repository.Save(tag, new Tag("vocaloud"));

			releaseEvent = repository.Save(CreateEntry.ReleaseEvent("Comiket 39"));

			permissionContext = new FakePermissionContext(user);
			entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			newSongContract = new CreateSongContract
			{
				SongType = SongType.Original,
				Names = new[] {
					new LocalizedStringContract("Resistance", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistForSongContract { Artist = new ArtistContract(producer, ContentLanguagePreference.Default) },
					new ArtistForSongContract { Artist = new ArtistContract(vocalist, ContentLanguagePreference.Default) },
				},
				PVUrls = new[] { "http://test.vocadb.net/" }
			};

			pvParser = new FakePVParser();
			pvParser.ResultFunc = (url, getMeta) =>
				VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939",
				getMeta ? VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", null, "testimg.jpg", 39) : VideoTitleParseResult.Empty);

			mailer = new FakeUserMessageMailer();

			queries = new SongQueries(repository, permissionContext, entryLinkFactory, pvParser, mailer,
				new FakeLanguageDetector(), new FakeUserIconFactory(), new EnumTranslations(), new InMemoryImagePersister(), new FakeObjectCache(), new Model.Utils.Config.VdbConfigManager(), new EntrySubTypeNameFactory(),
				new FollowedArtistNotifier(new FakeEntryLinkFactory(), mailer, new EnumTranslations(), new EntrySubTypeNameFactory()));
		}

		[TestMethod]
		public async Task Create()
		{
			var result = await CallCreate();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Resistance", result.Name, "Name");

			song = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			Assert.IsNotNull(song, "Song was saved to repository");
			Assert.AreEqual("Resistance", song.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, song.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, song.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(song.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", song.ArtistString.Default, "ArtistString");
			Assert.AreEqual(39, song.LengthSeconds, "Length");  // From PV
			Assert.AreEqual(PVServices.NicoNicoDouga, song.PVServices, "PVServices");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(song, archivedVersion.Song, "Archived version song");
			Assert.AreEqual(SongArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(song, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

			var pv = repository.List<PVForSong>().FirstOrDefault(p => p.Song.Id == song.Id);

			Assert.IsNotNull(pv, "PV was created");
			Assert.AreEqual(song, pv.Song, "pv.Song");
			Assert.AreEqual("Resistance", pv.Name, "pv.Name");
		}

		[TestMethod]
		public void Create_Notification()
		{
			repository.Save(user2.AddArtist(producer));

			CallCreate();

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.Receiver, "Receiver");
		}

		[TestMethod]
		public void Create_NoNotificationForSelf()
		{
			repository.Save(user.AddArtist(producer));

			CallCreate();

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification was created");
		}

		[TestMethod]
		public void Create_EmailNotification()
		{
			var subscription = repository.Save(user2.AddArtist(producer));
			subscription.EmailNotifications = true;

			CallCreate();

			var notification = repository.List<UserMessage>().First();

			Assert.AreEqual(notification.Subject, mailer.Subject, "Subject");
			Assert.IsNotNull(mailer.Body, "Body");
			Assert.AreEqual(notification.Receiver.Name, mailer.ReceiverName, "ReceiverName");
		}

		[TestMethod]
		public void Create_NotificationForTags()
		{
			repository.Save(user2.AddTag(tag));
			repository.Save(new TagMapping(tag, "VOCAROCK"));
			pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "VOCAROCK" });

			CallCreate();

			var notification = repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.User, "Notified user");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission()
		{
			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			CallCreate();
		}

		[TestMethod]
		public void Create_Tags()
		{
			repository.Save(new TagMapping(tag, "VOCAROCK"));
			pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "VOCAROCK" });

			CallCreate();

			song = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			Assert.AreEqual(1, song.Tags.Tags.Count(), "Tags.Count");
			Assert.IsTrue(song.Tags.HasTag(tag), "Has vocarock tag");
		}

		[TestMethod]
		public void Create_Tags_IgnoreDuplicates()
		{
			repository.Save(user2.AddTag(tag));
			repository.Save(new TagMapping(tag, "VOCAROCK"));
			repository.Save(new TagMapping(tag, "rock"));
			pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "VOCAROCK", "rock" });

			CallCreate();

			song = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			Assert.AreEqual(1, song.Tags.Tags.Count(), "Tags.Count");
			Assert.IsTrue(song.Tags.HasTag(tag), "Has vocarock tag");
			Assert.AreEqual(1, song.Tags.GetTagUsage(tag).Count, "Tag vote count");
			var messages = repository.List<UserMessage>().Where(u => u.User.Equals(user2)).ToArray();
			Assert.AreEqual(1, messages.Length, "Notification was sent");
			var message = messages[0];
			Assert.AreEqual(user2, message.Receiver, "Message receiver");
			Assert.AreEqual("New song tagged with vocarock", message.Subject, "Message subject"); // Test subject to make sure it's for one tag
		}

		[TestMethod]
		public async Task Create_Tags_IgnoreCoverIfSongTypeIsCover()
		{
			var coverTag = repository.Save(CreateEntry.Tag("cover"));
			repository.Save(new TagMapping(coverTag, "cover"));
			repository.Save(new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Song, SongType.Cover), coverTag));

			pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "cover" });
			newSongContract.SongType = SongType.Cover;

			var id = (await CallCreate()).Id;

			song = repository.Load(id);

			Assert.AreEqual(SongType.Cover, song.SongType, "SongType");
			Assert.AreEqual(0, song.Tags.Tags.Count(), "No tags added");
		}

		[TestMethod]
		public async Task Create_NoPV()
		{
			newSongContract.PVUrls = new string[0];

			var result = await CallCreate();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(PVServices.Nothing, result.PVServices, "PVServices");
		}

		[TestMethod]
		public async Task Create_EmptyPV()
		{
			newSongContract.PVUrls = new[] { string.Empty };

			var result = await CallCreate();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(PVServices.Nothing, result.PVServices, "PVServices");
		}

		/// <summary>
		/// Basic report, no existing reports.
		/// </summary>
		[TestMethod]
		public void CreateReport()
		{
			var (created, report) = CallCreateReport(SongReportType.InvalidInfo);

			Assert.IsTrue(created, "Report was created");
			Assert.AreEqual(song.Id, report.EntryBase.Id, "Entry Id");
			Assert.AreEqual(user, report.User, "Report author");
			Assert.AreEqual(SongReportType.InvalidInfo, report.ReportType, "Report type");
		}

		/// <summary>
		/// Report specific version.
		/// </summary>
		[TestMethod]
		public void CreateReport_Version()
		{
			var version = ArchivedSongVersion.Create(song, new SongDiff(), new AgentLoginData(user), SongArchiveReason.PropertiesUpdated, String.Empty);
			repository.Save(version);
			CallCreateReport(SongReportType.Other, version.Version);

			var report = repository.List<SongReport>().First();

			Assert.AreEqual(version.Version, report.VersionNumber, "Version number");
			Assert.IsNotNull(report.VersionBase, "VersionBase");

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user, notification.Receiver, "Notification receiver");
			Assert.AreEqual(string.Format(EntryReportStrings.EntryVersionReportTitle, song.DefaultName), notification.Subject, "Notification subject");
			Assert.AreEqual(string.Format(EntryReportStrings.EntryVersionReportBody,
				MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(song), song.DefaultName), "It's Miku, not Rin"),
				notification.Message, "Notification message");
		}

		/// <summary>
		/// Open report exists for another entry.
		/// </summary>
		[TestMethod]
		public void CreateReport_OtherEntry()
		{
			var song2 = repository.Save(CreateEntry.Song());
			CallCreateReport(SongReportType.InvalidInfo, song: song2);
			var secondResult = CallCreateReport(SongReportType.Other);

			var reports = repository.List<SongReport>();

			Assert.AreEqual(2, reports.Count, "Number of reports");
			Assert.IsTrue(secondResult.created, "Second report was created");
		}

		/// <summary>
		/// Duplicate report exists: skip.
		/// </summary>
		[TestMethod]
		public void CreateReport_Duplicate()
		{
			var (_, report) = CallCreateReport(SongReportType.InvalidInfo);
			var secondResult = CallCreateReport(SongReportType.Other);

			var reports = repository.List<SongReport>();

			Assert.AreEqual(1, reports.Count, "Number of reports");
			Assert.AreEqual(SongReportType.InvalidInfo, report.ReportType, "Report type");
			Assert.IsFalse(secondResult.created, "Second report was not created");
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

			var reports = repository.List<SongReport>();

			Assert.AreEqual(2, reports.Count, "Number of reports");
			Assert.IsTrue(secondCreated, "Second report was created");
		}

		/// <summary>
		/// Duplicate report exists. It is closed, but current user is not logged in. Skip.
		/// </summary>
		[TestMethod]
		public void CreateReport_Duplicate_Closed_NotLoggedIn()
		{
			permissionContext.LoggedUser = null;
			var (_, report) = CallCreateReport(SongReportType.InvalidInfo);
			report.Status = ReportStatus.Closed;
			var (secondCreated, _) = CallCreateReport(SongReportType.Other);

			var reports = repository.List<SongReport>();

			Assert.AreEqual(1, reports.Count, "Number of reports");
			Assert.IsFalse(secondCreated, "Second report was not created");
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

			var reports = repository.List<SongReport>();

			reports.Should().HaveCount(2);
			Assert.IsFalse(thirdCreated, "Third report was not created");
		}

		/// <summary>
		/// Create report when not logged in.
		/// Report is created using hostname.
		/// </summary>
		[TestMethod]
		public void CreateReport_NotLoggedIn()
		{
			permissionContext.LoggedUser = null;
			CallCreateReport(SongReportType.Other);

			var report = repository.List<SongReport>().FirstOrDefault();

			Assert.IsNotNull(report, "Report was created");
			Assert.IsNull(report.User, "User is null");
			Assert.AreEqual("39.39.39.39", report.Hostname, "Hostname");
		}

		// Create report, notify the user who created the entry if they're the only editor.
		[TestMethod]
		public void CreateReport_NotifyIfUnambiguous()
		{
			var editor = user2;
			repository.Save(ArchivedSongVersion.Create(song, new SongDiff(), new AgentLoginData(editor), SongArchiveReason.PropertiesUpdated, String.Empty));
			queries.CreateReport(song.Id, SongReportType.Other, "39.39.39.39", "It's Miku, not Rin", null);

			var report = repository.List<SongReport>().First();
			Assert.AreEqual(song, report.Entry, "Report was created for song");
			Assert.IsNull(report.Version, "Version");

			var notification = repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNotNull(notification, "notification was created");
			Assert.AreEqual(editor, notification.Receiver, "notification was receiver is editor");
			Assert.AreEqual(string.Format(EntryReportStrings.EntryVersionReportTitle, song.DefaultName), notification.Subject, "Notification subject");
		}

		[TestMethod]
		public void CreateReport_DoNotNotifyIfAmbiguous()
		{
			repository.Save(ArchivedSongVersion.Create(song, new SongDiff(), new AgentLoginData(user2), SongArchiveReason.PropertiesUpdated, String.Empty));
			repository.Save(ArchivedSongVersion.Create(song, new SongDiff(), new AgentLoginData(user3), SongArchiveReason.PropertiesUpdated, String.Empty));
			queries.CreateReport(song.Id, SongReportType.Other, "39.39.39.39", "It's Miku, not Rin", null);

			var report = repository.List<SongReport>().First();
			Assert.AreEqual(song, report.Entry, "Report was created for song");
			Assert.IsNull(report.Version, "Version");

			var notification = repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNull(notification, "notification was not created");
		}

		// Create report, with both report type name and notes
		[TestMethod]
		public void CreateReport_Notify_ReportTypeNameAndNotes()
		{
			var editor = user2;
			repository.Save(ArchivedSongVersion.Create(song, new SongDiff(), new AgentLoginData(editor), SongArchiveReason.PropertiesUpdated, String.Empty));
			queries.CreateReport(song.Id, SongReportType.BrokenPV, "39.39.39.39", "It's Miku, not Rin", null);

			var entryLink = MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(song), song.DefaultName);

			var notification = repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNotNull(notification, "notification was created");
			Assert.AreEqual(string.Format(EntryReportStrings.EntryVersionReportTitle, song.DefaultName), notification.Subject, "Notification subject");
			Assert.AreEqual(string.Format(EntryReportStrings.EntryVersionReportBody, entryLink, "Broken PV (It's Miku, not Rin)"), notification.Message, "Notification body");
		}


		// Two PVs, no matches, parse song info from the NND PV.
		[TestMethod]
		public async Task FindDuplicates_NoMatches_ParsePVInfo()
		{
			// Note: Nico will be preferred, if available
			pvParser.MatchedPVs.Add("http://youtu.be/123456567",
				VideoUrlParseResult.CreateOk("http://youtu.be/123456567", PVService.Youtube, "123456567",
				VideoTitleParseResult.CreateSuccess("anger PV", "Tripshots", null, "testimg2.jpg", 33)));

			pvParser.MatchedPVs.Add("http://www.nicovideo.jp/watch/sm3183550",
				VideoUrlParseResult.CreateOk("http://www.nicovideo.jp/watch/sm3183550", PVService.NicoNicoDouga, "sm3183550",
				VideoTitleParseResult.CreateSuccess("【初音ミク】anger【VOCALOID3DPV】", "Tripshots", null, "testimg.jpg", 39)));

			var result = await CallFindDuplicates(new[] { "【初音ミク】anger【VOCALOID3DPV】" }, new[] { "http://youtu.be/123456567", "http://www.nicovideo.jp/watch/sm3183550" });

			Assert.AreEqual("anger", result.Title, "Title"); // Title from PV
			Assert.AreEqual(0, result.Matches.Length, "No matches");
		}

		[TestMethod]
		public async Task FindDuplicates_ParsePVInfo_YouTube()
		{
			var artist = repository.Save(CreateEntry.Artist(ArtistType.Producer, name: "Clean Tears"));
			repository.Save(artist.CreateWebLink("YouTube", "https://www.youtube.com/channel/UCnHGCQ0pwnRFF5Oe2YTeOcA", WebLinkCategory.Official));

			var titleParseResult = VideoTitleParseResult.CreateSuccess("Clean Tears - Ruby", "Clean Tears", null, "http://tn.smilevideo.jp/smile?i=32347786", 39);
			titleParseResult.Author = "Clean Tears";
			titleParseResult.AuthorId = "UCnHGCQ0pwnRFF5Oe2YTeOcA";

			pvParser.MatchedPVs.Add("https://youtu.be/aJKY_EeAeYc",
				VideoUrlParseResult.CreateOk("https://youtu.be/aJKY_EeAeYc", PVService.Youtube, "aJKY_EeAeYc", titleParseResult));

			var result = await CallFindDuplicates(new string[0], new[] { "https://youtu.be/aJKY_EeAeYc" });

			Assert.AreEqual("Clean Tears - Ruby", result.Title, "Title"); // Title from PV
			Assert.AreEqual(1, result.Artists.Length, "Number of matched artists");
			Assert.AreEqual(artist.Id, result.Artists[0].Id, "Matched artist");
		}

		[TestMethod]
		public async Task FindDuplicates_MatchName()
		{
			var result = await CallFindDuplicates(new[] { "Nebula" });

			Assert.AreEqual(1, result.Matches.Length, "Matches");
			var match = result.Matches.First();
			Assert.AreEqual(song.Id, match.Entry.Id, "Matched song");
			Assert.AreEqual(SongMatchProperty.Title, match.MatchProperty, "Matched property");
		}

		[TestMethod]
		public async Task FindDuplicates_MatchNameAndArtist()
		{
			var producer2 = Save(CreateEntry.Artist(ArtistType.Producer, name: "minato"));
			var song2 = repository.Save(CreateEntry.Song(name: "Nebula"));
			Save(song2.AddArtist(producer2));

			var result = await CallFindDuplicates(new[] { "Nebula" }, artistIds: new[] { producer2.Id });

			// 2 songs, the one with both artist and title match appears first
			Assert.AreEqual(2, result.Matches.Length, "Matches");
			var match = result.Matches.First();
			Assert.AreEqual(song2.Id, match.Entry.Id, "Matched song");
			Assert.AreEqual(SongMatchProperty.Title, match.MatchProperty, "Matched property");
		}

		[TestMethod]
		public async Task FindDuplicates_SkipPVInfo()
		{
			var result = await CallFindDuplicates(new[] { "Anger" }, new[] { "http://www.nicovideo.jp/watch/sm393939" }, getPvInfo: false);

			Assert.IsNull(result.Title, "Title");
			Assert.AreEqual(0, result.Matches.Length, "No matches");
		}

		[TestMethod]
		public async Task FindDuplicates_MatchPV()
		{
			pvParser.MatchedPVs.Add("http://youtu.be/hoLu7c2XZYU",
				VideoUrlParseResult.CreateOk("http://youtu.be/hoLu7c2XZYU", PVService.Youtube, "hoLu7c2XZYU", VideoTitleParseResult.Empty));

			var result = await CallFindDuplicates(anyPv: new[] { "http://youtu.be/hoLu7c2XZYU" });

			Assert.AreEqual(1, result.Matches.Length, "Matches");
			var match = result.Matches.First();
			Assert.AreEqual(song.Id, match.Entry.Id, "Matched song");
			Assert.AreEqual(SongMatchProperty.PV, match.MatchProperty, "Matched property");
		}

		[TestMethod]
		public async Task FindDuplicates_CoverInSongTitle_CoverType()
		{
			pvParser.MatchedPVs.Add("http://www.nicovideo.jp/watch/sm27114783",
				VideoUrlParseResult.CreateOk("http://www.nicovideo.jp/watch/sm27114783", PVService.NicoNicoDouga, "123456567",
				VideoTitleParseResult.CreateSuccess("【GUMI】 光(宇多田ヒカル) 【アレンジカバー】", string.Empty, null, "testimg2.jpg", 33)));

			var result = await CallFindDuplicates(anyPv: new[] { "http://www.nicovideo.jp/watch/sm27114783" });

			Assert.AreEqual(SongType.Cover, result.SongType, "SongType is cover because of the 'cover' in title");
		}

		[TestMethod]
		public void GetRelatedSongs()
		{
			var matchingArtist = Save(CreateEntry.Song());
			Save(matchingArtist.AddArtist(song.Artists.First().Artist));

			Save(song.AddTag(tag).Result);
			var matchingTag = Save(CreateEntry.Song());
			Save(matchingTag.AddTag(tag).Result);

			Save(user.AddSongToFavorites(song, SongVoteRating.Like));
			var matchingLike = Save(CreateEntry.Song());
			Save(user.AddSongToFavorites(matchingLike, SongVoteRating.Like));

			// Unrelated song
			Save(CreateEntry.Song());

			var result = queries.GetRelatedSongs(song.Id, SongOptionalFields.AdditionalNames);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.ArtistMatches.Length, "Number of artist matches");
			Assert.AreEqual(matchingArtist.Id, result.ArtistMatches.First().Id, "Matching artist");
			Assert.AreEqual(1, result.TagMatches.Length, "Number of tag matches");
			Assert.AreEqual(matchingTag.Id, result.TagMatches.First().Id, "Matching tag");
			Assert.AreEqual(1, result.LikeMatches.Length, "Number of like matches");
			Assert.AreEqual(matchingLike.Id, result.LikeMatches.First().Id, "Matching like");
		}

		[TestMethod]
		public void GetSongForEdit()
		{
			var album = repository.Save(CreateEntry.Album());
			album.OriginalRelease.ReleaseDate = new OptionalDateTime(2007, 8, 31);
			var relEvent = repository.Save(CreateEntry.ReleaseEvent("Miku's birthday", new DateTime(2007, 8, 31)));
			album.OriginalRelease.ReleaseEvent = relEvent;
			album.AddSong(song, 1, 1);

			var album2 = repository.Save(CreateEntry.Album());
			album2.OriginalRelease.ReleaseDate = new OptionalDateTime(2017, 8, 31);
			album2.AddSong(song, 1, 2);

			var result = queries.GetSongForEdit(song.Id);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(relEvent.Id, result.AlbumEventId, "AlbumEventId");
		}

		[TestMethod]
		public async Task GetTagSuggestions()
		{
			var tag2 = repository.Save(CreateEntry.Tag("metalcore"));
			repository.Save(new TagMapping(tag, "vocarock"));
			repository.Save(new TagMapping(tag2, "vocacore"));

			pvParser.ResultFunc = (url, getMeta) =>
				VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939",
					getMeta ? VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", null, "testimg.jpg", tags: new[] { "vocarock", "vocacore" }) : VideoTitleParseResult.Empty);

			song.AddTag(tag);
			song.PVs.Add(new PVForSong(song, new PVContract { Service = PVService.NicoNicoDouga, PVType = PVType.Original, PVId = "sm393939" }));

			var result = await queries.GetTagSuggestionsAsync(song.Id);

			Assert.AreEqual(1, result.Count, "One suggestion");
			Assert.AreEqual("metalcore", result[0].Tag.Name, "Tag name");
		}

		[TestMethod]
		public async Task GetTagSuggestions_IgnoreCoverTagIfTypeIsCover()
		{
			var coverTag = repository.Save(CreateEntry.Tag("cover"));
			repository.Save(new TagMapping(coverTag, "cover"));
			repository.Save(new EntryTypeToTagMapping(EntryTypeAndSubType.Create(EntryType.Song, SongType.Cover), coverTag));

			pvParser.ResultFunc = (url, meta) => CreateEntry.VideoUrlParseResultWithTitle(tags: new[] { "cover" });
			song.SongType = SongType.Cover;
			song.PVs.Add(new PVForSong(song, new PVContract { Service = PVService.NicoNicoDouga, PVType = PVType.Original, PVId = "sm393939" }));

			var result = await queries.GetTagSuggestionsAsync(song.Id);

			Assert.AreEqual(0, result.Count, "Cover tag suggestion ignored");
		}

		[TestMethod]
		public async Task GetTagSuggestions_ShortVersion()
		{
			var shortVersionTag = repository.Save(CreateEntry.Tag("short version", shortVersionTagId));

			song.LengthSeconds = 3939;

			var remix = repository.Save(CreateEntry.Song());
			remix.SongType = SongType.Remix;
			remix.LengthSeconds = 39;
			remix.SetOriginalVersion(song);

			var result = await queries.GetTagSuggestionsAsync(remix.Id);

			Assert.AreEqual("short version", result.FirstOrDefault()?.Tag.Name, "Short version tag was returned");
		}

		[TestMethod]
		public void Merge_ToEmpty()
		{
			song.Notes.Original = "Notes";
			var song2 = new Song();
			repository.Save(song2);

			queries.Merge(song.Id, song2.Id);

			Assert.AreEqual("Nebula", song2.Names.AllValues.FirstOrDefault(), "Name");
			Assert.AreEqual(2, song2.AllArtists.Count, "Artists");
			AssertHasArtist(song2, producer);
			AssertHasArtist(song2, vocalist);
			Assert.AreEqual(song.LengthSeconds, song2.LengthSeconds, "LengthSeconds");
			Assert.AreEqual(song.Notes.Original, song2.Notes.Original, "Notes were copied");

			var mergeRecord = repository.List<SongMergeRecord>().FirstOrDefault();
			Assert.IsNotNull(mergeRecord, "Merge record was created");
			Assert.AreEqual(song.Id, mergeRecord.Source, "mergeRecord.Source");
			Assert.AreEqual(song2.Id, mergeRecord.Target.Id, "mergeRecord.Target.Id");
		}

		[TestMethod]
		public void Merge_WithArtists()
		{
			song.GetArtistLink(producer).Roles = ArtistRoles.Instrumentalist;
			var song2 = CreateEntry.Song();
			repository.Save(song2);
			song2.AddArtist(vocalist);
			song2.AddArtist(vocalist2).Roles = ArtistRoles.Other;

			queries.Merge(song.Id, song2.Id);
			Assert.AreEqual(3, song2.AllArtists.Count, "Artists");
			AssertHasArtist(song2, producer, ArtistRoles.Instrumentalist);
			AssertHasArtist(song2, vocalist2, ArtistRoles.Other);
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Merge_NoPermissions()
		{
			user.GroupId = UserGroupId.Regular;
			permissionContext.RefreshLoggedUser(repository);

			var song2 = new Song();
			repository.Save(song2);

			queries.Merge(song.Id, song2.Id);
		}

		[TestMethod]
		public async Task Revert()
		{
			user.GroupId = UserGroupId.Moderator;
			permissionContext.RefreshLoggedUser(repository);
			SongForEditContract Contract()
			{
				return new SongForEditContract(song, ContentLanguagePreference.English);
			}

			await queries.UpdateBasicProperties(Contract());

			// Remove all artists
			var contract = Contract();
			contract.Artists = new ArtistForSongContract[0];
			await queries.UpdateBasicProperties(contract);

			var latestVersionBeforeRevert = song.ArchivedVersionsManager.GetLatestVersion();
			Assert.IsNotNull(latestVersionBeforeRevert, "latestVersion");
			Assert.AreEqual(2, song.Version, "Version number before revert");

			queries.RevertToVersion(latestVersionBeforeRevert.Id);

			Assert.AreEqual(3, song.Version, "Version was incremented");
			Assert.AreEqual(2, song.Artists.Count(), "Artist links were restored");
			Assert.AreNotEqual(song.ArtistString?.Default, string.Empty, "Artist string was restored");

			var latestVersion = song.ArchivedVersionsManager.GetLatestVersion();
			Assert.AreEqual(SongArchiveReason.Reverted, latestVersion.Reason, "Reason");
			Assert.IsTrue(latestVersion.IsIncluded(SongEditableFields.Artists), "Artists are included in diff");
		}

		[TestMethod]
		public async Task Update_Names()
		{
			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.Names.First().Value = "Replaced name";
			contract.UpdateNotes = "Updated song";

			contract = await queries.UpdateBasicProperties(contract);

			var songFromRepo = repository.Load(contract.Id);
			Assert.AreEqual("Replaced name", songFromRepo.DefaultName);
			Assert.AreEqual(1, songFromRepo.Version, "Version");
			Assert.AreEqual(2, songFromRepo.AllArtists.Count, "Number of artists");
			Assert.AreEqual(0, songFromRepo.AllAlbums.Count, "No albums");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(song, archivedVersion.Song, "Archived version song");
			Assert.AreEqual(SongArchiveReason.PropertiesUpdated, archivedVersion.Reason, "Archived version reason");
			Assert.AreEqual(SongEditableFields.Names, archivedVersion.Diff.ChangedFields.Value, "Changed fields");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(song, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Updated, activityEntry.EditEvent, "Activity entry event type");
		}

		[TestMethod]
		public async Task Update_Artists()
		{
			var newSong = CreateEntry.Song(name: "Anger");

			repository.Save(newSong);

			foreach (var name in newSong.Names)
				repository.Save(name);

			var contract = new SongForEditContract(newSong, ContentLanguagePreference.English);
			contract.Artists = new[] {
				CreateArtistForSongContract(artistId: producer.Id),
				CreateArtistForSongContract(artistId: vocalist.Id),
				CreateArtistForSongContract(artistName: "Goomeh", roles: ArtistRoles.Vocalist),
			};

			contract = await queries.UpdateBasicProperties(contract);

			var songFromRepo = repository.Load(contract.Id);

			Assert.AreEqual(3, songFromRepo.AllArtists.Count, "Number of artists");

			AssertHasArtist(songFromRepo, producer);
			AssertHasArtist(songFromRepo, vocalist);
			Assert.AreEqual("Tripshots feat. Hatsune Miku, Goomeh", songFromRepo.ArtistString.Default, "Artist string");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(SongEditableFields.Artists, archivedVersion.Diff.ChangedFields.Value, "Changed fields");
		}

		[TestMethod]
		public async Task Update_Artists_Notify()
		{
			repository.Save(user2.AddArtist(vocalist2));
			repository.Save(vocalist2);

			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.Artists = contract.Artists.Concat(new[] { CreateArtistForSongContract(vocalist2.Id) }).ToArray();

			await queries.UpdateBasicProperties(contract);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.Receiver, "Receiver");
		}

		[TestMethod]
		public async Task Update_Artists_RemoveDeleted()
		{
			repository.Save(vocalist2);
			repository.Save(song.AddArtist(vocalist2));
			vocalist2.Deleted = true;

			var contract = new SongForEditContract(song, ContentLanguagePreference.English);

			await queries.UpdateBasicProperties(contract);

			Assert.IsFalse(song.AllArtists.Any(a => Equals(vocalist2, a.Artist)), "vocalist2 was removed from song");
		}

		[TestMethod]
		public async Task Update_Lyrics()
		{
			var contract = EditContract();
			contract.Lyrics = new[] {
				CreateEntry.LyricsForSongContract(cultureCode: OptionalCultureCode.LanguageCode_English, translationType: TranslationType.Original)
			};

			await queries.UpdateBasicProperties(contract);

			Assert.AreEqual(1, song.Lyrics.Count, "Lyrics were added");
			var lyrics = song.Lyrics.First();
			Assert.AreEqual("Miku Miku", lyrics.Value, "Lyrics text");
		}

		[TestMethod]
		public async Task Update_PublishDate_From_PVs()
		{
			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.PVs = new[] {
				CreateEntry.PVContract(id: 1, pvId: "hoLu7c2XZYU", pvType: PVType.Reprint, publishDate: new DateTime(2015, 3, 9, 10, 0, 0)),
				CreateEntry.PVContract(id: 2, pvId: "mikumikumiku", pvType: PVType.Original, publishDate: new DateTime(2015, 4, 9, 16, 0, 0))
			};

			contract = await queries.UpdateBasicProperties(contract);

			var songFromRepo = repository.Load(contract.Id);
			Assert.AreEqual(2, songFromRepo.PVs.PVs.Count, "Number of PVs");
			Assert.AreEqual(new DateTime(2015, 4, 9), songFromRepo.PublishDate.DateTime, "Song publish date was updated");
		}

		[TestMethod]
		public async Task Update_Weblinks()
		{
			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.WebLinks = new[] {
				new WebLinkContract("http://vocadb.net", "VocaDB", WebLinkCategory.Reference)
			};

			contract = await queries.UpdateBasicProperties(contract);
			var songFromRepo = repository.Load(contract.Id);
			Assert.AreEqual(1, songFromRepo.WebLinks.Count, "Number of weblinks");
			Assert.AreEqual("http://vocadb.net", songFromRepo.WebLinks[0].Url, "Weblink URL");
		}

		[TestMethod]
		public async Task Update_Weblinks_SkipWhitespace()
		{
			var contract = new SongForEditContract(song, ContentLanguagePreference.English);
			contract.WebLinks = new[] {
				new WebLinkContract(" ", "VocaDB", WebLinkCategory.Reference)
			};

			contract = await queries.UpdateBasicProperties(contract);
			var songFromRepo = repository.Load(contract.Id);
			Assert.AreEqual(0, songFromRepo.WebLinks.Count, "Number of weblinks");
		}

		/// <summary>
		/// User has selected the event
		/// </summary>
		[TestMethod]
		public async Task Update_ReleaseEvent_ExistingEvent_Selected()
		{
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract(releaseEvent, ContentLanguagePreference.English);

			await queries.UpdateBasicProperties(contract);

			Assert.AreSame(releaseEvent, song.ReleaseEvent, "ReleaseEvent");
		}

		/// <summary>
		/// User typed an event name, and there's a name match
		/// </summary>
		[TestMethod]
		public async Task Update_ReleaseEvent_ExistingEvent_MatchByName()
		{
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract { Name = releaseEvent.DefaultName };

			await queries.UpdateBasicProperties(contract);

			Assert.AreSame(releaseEvent, song.ReleaseEvent, "ReleaseEvent");
		}

		[TestMethod]
		public async Task Update_ReleaseEvent_NewEvent_Standalone()
		{
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract { Name = "Comiket 40" };

			await queries.UpdateBasicProperties(contract);

			Assert.IsNotNull(song.ReleaseEvent, "ReleaseEvent");
			Assert.AreSame("Comiket 40", song.ReleaseEvent.DefaultName, "ReleaseEvent.Name");

			Assert.AreEqual(1, song.ReleaseEvent.ArchivedVersionsManager.Versions.Count, "New release event was archived");
		}

		[TestMethod]
		public async Task Update_ReleaseEvent_NewEvent_SeriesEvent()
		{
			var series = repository.Save(CreateEntry.EventSeries("Comiket"));
			var contract = EditContract();
			contract.ReleaseEvent = new ReleaseEventContract { Name = "Comiket 40" };

			await queries.UpdateBasicProperties(contract);

			Assert.IsNotNull(song.ReleaseEvent, "ReleaseEvent");
			Assert.AreEqual(series, song.ReleaseEvent.Series, "Series");
			Assert.AreEqual(40, song.ReleaseEvent.SeriesNumber, "SeriesNumber");
		}

		[TestMethod]
		public async Task Update_SendNotificationsForNewPVs()
		{
			song.PVs.PVs.Clear();
			song.CreateDate = DateTime.Now - TimeSpan.FromDays(30);
			repository.Save(user2.AddArtist(producer));
			var contract = EditContract();
			contract.PVs = new[] { CreateEntry.PVContract(pvType: PVType.Original) };

			await queries.UpdateBasicProperties(contract);

			var notifications = repository.List<UserMessage>();
			Assert.AreEqual(1, notifications.Count, "Notification was sent");
			var notification = notifications.First();
			Assert.AreEqual(user2, notification.User, "Notification was sent to user");
		}

		[TestMethod]
		public async Task Update_DoNotSendNotificationsForReprints()
		{
			song.PVs.PVs.Clear();
			song.CreateDate = DateTime.Now - TimeSpan.FromDays(30);
			repository.Save(user2.AddArtist(producer));
			var contract = EditContract();
			contract.PVs = new[] { CreateEntry.PVContract(pvType: PVType.Reprint) };

			await queries.UpdateBasicProperties(contract);

			Assert.AreEqual(0, repository.Count<UserMessage>(), "No notification was sent");
		}
	}
}
