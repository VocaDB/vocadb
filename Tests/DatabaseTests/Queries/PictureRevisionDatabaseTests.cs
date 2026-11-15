#nullable disable

using System.Net.Mime;
using Autofac;
using NHibernate;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries;

[TestClass]
public class PictureRevisionDatabaseTests
{
	// Uses the test's session, another connection would need a distributed transaction
	private class AlbumSessionPerRequestRepository : NHibernateSessionPerRequestRepository<Album>, IAlbumRepository
	{
		public AlbumSessionPerRequestRepository(ISession session, ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(session, sessionFactory, permissionContext) { }
	}

	private readonly DatabaseTestContext<ISession> _context = new();
	private readonly InMemoryImagePersister _imageStore = new();
	private readonly FakePermissionContext _userContext;
	private TestDatabase Db => TestContainerManager.TestDatabase;

	public PictureRevisionDatabaseTests()
	{
		_userContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
		_userContext.GrantPermission(PermissionToken.ViewCoverArtImages);
		_userContext.GrantPermission(PermissionToken.RestoreRevisions);
	}

	private IAlbumRepository CreateRepository(ISession session)
	{
		return new AlbumSessionPerRequestRepository(session, TestContainerManager.Container.Resolve<ISessionFactory>(), _userContext);
	}

	private AlbumQueries CreateQueries(IAlbumRepository repository)
	{
		return new AlbumQueries(
			repository,
			_userContext,
			new FakeEntryLinkFactory(),
			_imageStore,
			_imageStore,
			new FakeUserMessageMailer(),
			new FakeUserIconFactory(),
			new EnumTranslations(),
			new FakePVParser(),
			new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()),
			_imageStore,
			new FakeObjectCache(),
			new FakeDiscordWebhookNotifier()
		);
	}

	private static async Task UpdatePicture(AlbumQueries queries, int albumId, Stream image)
	{
		var contract = queries.GetForEdit(albumId);
		using var stream = image;
		await queries.UpdateBasicProperties(contract, new EntryPictureFileContract(stream, MediaTypeNames.Image.Jpeg, (int)stream.Length, ImagePurpose.Main));
	}

	/// <summary>
	/// Reads the revisions from the database, not the session.
	/// </summary>
	private static (int? Album, int? LatestArchived) GetRevisions(IAlbumRepository repository, int albumId)
	{
		return repository.HandleQuery(ctx => (
			ctx.Query().Where(a => a.Id == albumId).Select(a => a.CoverPictureRevision).Single(),
			ctx.Query<ArchivedAlbumVersion>().Where(v => v.Album.Id == albumId).OrderByDescending(v => v.Version).Select(v => v.CoverPictureRevision).First()
		));
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public Task UpdateBasicProperties_Picture()
	{
		return _context.RunTestAsync(async session =>
		{
			var repository = CreateRepository(session);
			var queries = CreateQueries(repository);
			var albumId = Db.Album3.Id;
			var version = repository.HandleQuery(ctx => ctx.Query().Where(a => a.Id == albumId).Select(a => a.Version).Single());

			await UpdatePicture(queries, albumId, ResourceHelper.TestImage());

			GetRevisions(repository, albumId).Should().Be(((int?)version, (int?)version), "Revision of album and archived version");
			var url = queries.GetArchivedCoverPictureUrl(repository.HandleQuery(ctx => ctx.Query<ArchivedAlbumVersion>().Where(v => v.Album.Id == albumId).OrderByDescending(v => v.Version).First().Id));
			url.Should().NotBeNull("Archived picture URL");
			url.Url.Should().Be($"https://static.vocadb.net/img/Album/{albumId}-{version}/Original", "Archived picture URL");
		});
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public Task RevertToVersion_PictureRevision()
	{
		return _context.RunTestAsync(async session =>
		{
			var repository = CreateRepository(session);
			var queries = CreateQueries(repository);
			var albumId = Db.Album3.Id;

			await UpdatePicture(queries, albumId, ResourceHelper.TestImage());
			var (firstRevision, _) = GetRevisions(repository, albumId);
			var firstVersionId = repository.HandleQuery(ctx => ctx.Query<ArchivedAlbumVersion>().Where(v => v.Album.Id == albumId && v.Version == firstRevision).Select(v => v.Id).First());
			await UpdatePicture(queries, albumId, ResourceHelper.TestImage2);
			GetRevisions(repository, albumId).Album.Should().Be(firstRevision + 1, "Second picture has a new revision");

			queries.RevertToVersion(firstVersionId);

			GetRevisions(repository, albumId).Should().Be((firstRevision, firstRevision), "Revert points to the first revision");
		});
	}
}
