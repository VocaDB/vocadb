#nullable disable

using Autofac;
using NHibernate;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries;

/// <summary>
/// Commits its own songs, because the tests need several connections.
/// </summary>
[TestClass]
public class LoadForUpdateDatabaseTests
{
	private readonly FakePermissionContext _userContext;
	private static ISessionFactory SessionFactory => TestContainerManager.Container.Resolve<ISessionFactory>();
	private TestDatabase Db => TestContainerManager.TestDatabase;

	public LoadForUpdateDatabaseTests()
	{
		_userContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
	}

	private static int CreateSong()
	{
		using var session = SessionFactory.OpenSession();
		using var tx = session.BeginTransaction();
		var song = new Song(new LocalizedString("LoadForUpdate " + Guid.NewGuid(), ContentLanguageSelection.English))
		{
			SongType = SongType.Original,
			CreateDate = DateTime.Now
		};
		session.Save(song);
		tx.Commit();
		return song.Id;
	}

	private static void IncrementVersionInDatabase(int songId)
	{
		using var session = SessionFactory.OpenSession();
		using var tx = session.BeginTransaction();
		session.CreateSQLQuery("update Songs set Version = Version + 1 where Id = :id").SetInt32("id", songId).ExecuteUpdate();
		tx.Commit();
	}

	private static int GetVersionFromDatabase(int songId)
	{
		using var session = SessionFactory.OpenSession();
		return session.CreateSQLQuery("select Version from Songs where Id = :id").SetInt32("id", songId).UniqueResult<int>();
	}

	private SongQueries Queries(ISongRepository repository)
	{
		return new SongQueries(
			repository,
			_userContext,
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
			new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()),
			new FakeDiscordWebhookNotifier()
		);
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public async Task LoadForUpdate_WaitsAndReadsCommittedVersion()
	{
		var songId = CreateSong();
		var version = GetVersionFromDatabase(songId);

		Task<int> second;
		using (var session = SessionFactory.OpenSession())
		using (var tx = session.BeginTransaction())
		{
			var song = session.LoadForUpdate<Song>(songId);
			song.Version.Should().Be(version, "First edit reads the current version");

			second = Task.Run(async () =>
			{
				using var otherSession = SessionFactory.OpenSession();
				using var otherTx = otherSession.BeginTransaction();
				var other = await otherSession.LoadForUpdateAsync<Song>(songId);
				var otherVersion = other.Version;
				await otherTx.CommitAsync();
				return otherVersion;
			});

			await Task.Delay(1000);
			second.IsCompleted.Should().BeFalse("Second edit waits for the first one");

			song.Version++;
			await tx.CommitAsync();
		}

		(await second.WaitAsync(TimeSpan.FromSeconds(30))).Should().Be(version + 1, "Second edit reads the version committed by the first one");
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public void LoadForUpdate_LoadedInSession_Refreshes()
	{
		var songId = CreateSong();
		using var session = SessionFactory.OpenSession();
		using var tx = session.BeginTransaction();
		var loaded = session.Get<Song>(songId);
		var staleVersion = loaded.Version;

		IncrementVersionInDatabase(songId);
		var song = session.LoadForUpdate<Song>(songId);

		song.Should().BeSameAs(loaded, "Same instance");
		song.Version.Should().Be(staleVersion + 1, "State was reloaded");
		tx.Rollback();
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public async Task UpdateBasicProperties_Concurrent_UniqueVersions()
	{
		const int edits = 8;
		var songId = CreateSong();
		var startVersion = GetVersionFromDatabase(songId);

		var contracts = Enumerable.Range(0, edits).Select(i =>
		{
			using var session = SessionFactory.OpenSession();
			var contract = new SongForEditForApiContract(session.Get<Song>(songId), ContentLanguagePreference.English, _userContext);
			contract.UpdateNotes = $"Edit {i}";
			return contract;
		}).ToArray();

		var queries = Queries(new SongNHibernateRepository(SessionFactory, _userContext));
		using var start = new ManualResetEventSlim();
		var tasks = contracts.Select(contract => Task.Run(async () =>
		{
			start.Wait();
			await queries.UpdateBasicProperties(contract);
		})).ToArray();
		start.Set();
		await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(60));

		using var check = SessionFactory.OpenSession();
		var versions = check.CreateSQLQuery("select Version from ArchivedSongVersions where Song = :id order by Version").SetInt32("id", songId).List<int>();
		versions.Should().Equal(Enumerable.Range(startVersion, edits), "One archived version per edit, numbered without gaps or duplicates");
		GetVersionFromDatabase(songId).Should().Be(startVersion + edits, "Song version");
	}
}
