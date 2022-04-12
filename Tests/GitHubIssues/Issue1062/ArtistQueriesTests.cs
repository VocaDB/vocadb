using System.Runtime.Caching;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue1062
{
	[TestClass]
	public class ArtistQueriesTests
	{
		private Artist _artist = default!;
		private InMemoryImagePersister _imagePersister = default!;
		private FakePermissionContext _permissionContext = default!;
		private ArtistQueries _queries = default!;
		private FakeArtistRepository _repository = default!;

		/// <summary>
		/// Logged in user
		/// </summary>
		private User _user = default!;

		[TestInitialize]
		public void SetUp()
		{
			_artist = CreateEntry.Producer(name: "Tripshots");
			_repository = new FakeArtistRepository(_artist);
			_repository.SaveNames(_artist);

			_user = CreateEntry.User(name: "Miku", group: UserGroupId.Moderator);
			_repository.Save(_user);

			_permissionContext = new FakePermissionContext(_user);
			_imagePersister = new InMemoryImagePersister();

			_queries = new ArtistQueries(
				_repository,
				_permissionContext,
				new FakeEntryLinkFactory(),
				_imagePersister,
				_imagePersister,
				MemoryCache.Default,
				new FakeUserIconFactory(),
				new EnumTranslations(),
				_imagePersister,
				new FakeDiscordWebhookNotifier()
			);
		}

		private Task<(bool created, int reportId)> CallCreateReport(
			ArtistReportType reportType,
			string notes = "It's Miku, not Rin",
			int? versionNumber = null
		)
		{
			return _queries.CreateReport(_artist.Id, reportType, "39.39.39.39", notes, versionNumber);
		}

		[TestMethod]
		public async Task CreateReport_InvalidInfo_WithNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.InvalidInfo);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_InvalidInfo_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.InvalidInfo, notes: string.Empty);

			created.Should().BeFalse("Report was not created");
		}

		[TestMethod]
		public async Task CreateReport_Duplicate_WithNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.Duplicate);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Duplicate_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.Duplicate, notes: string.Empty);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Inappropriate_WithNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.Inappropriate);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Inappropriate_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.Inappropriate, notes: string.Empty);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_OwnershipClaim_WithNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.OwnershipClaim);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_OwnershipClaim_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.OwnershipClaim, notes: string.Empty);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Other_WithNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.Other);

			created.Should().BeTrue("Report was created");
		}

		[TestMethod]
		public async Task CreateReport_Other_WithoutNotes()
		{
			var (created, _) = await CallCreateReport(ArtistReportType.Other, notes: string.Empty);

			created.Should().BeFalse("Report was not created");
		}
	}
}
