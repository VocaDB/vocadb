#nullable disable

using NHibernate;
using NLog;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.TagFormatting;

namespace VocaDb.Model.Service;

public class AlbumService : ServiceBase
{
	private readonly IEntryUrlParser _entryUrlParser;
	private readonly IUserIconFactory _userIconFactory;

	// ReSharper disable UnusedMember.Local
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	// ReSharper restore UnusedMember.Local

	private PartialFindResult<Album> Find(ISession session, AlbumQueryParams queryParams)
	{
		return new AlbumSearch(new NHibernateDatabaseContext(session, PermissionContext), queryParams.LanguagePreference, _entryUrlParser).Find(queryParams);
	}

	public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IUserIconFactory userIconFactory, IEntryUrlParser entryUrlParser)
		: base(sessionFactory, permissionContext, entryLinkFactory)
	{
		_userIconFactory = userIconFactory;
		_entryUrlParser = entryUrlParser;
	}

	public ArchivedAlbumVersion Archive(ISession session, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "")
	{
		var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
		var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
		session.Save(archived);
		return archived;
	}

	public void Archive(ISession session, Album album, AlbumArchiveReason reason, string notes = "")
	{
		Archive(session, album, new AlbumDiff(), reason, notes);
	}

	public void Delete(int id, string notes)
	{
		UpdateEntity<Album>(id, (session, a) =>
		{
			EntryPermissionManager.VerifyDelete(PermissionContext, a);

			AuditLog($"deleting album {EntryLinkFactory.CreateEntryLink(a)}", session);

			NHibernateUtil.Initialize(a.CoverPictureData);
			a.Delete();

			Archive(session, a, new AlbumDiff(false), AlbumArchiveReason.Deleted, notes);
		}, PermissionToken.Nothing, skipLog: true);
	}

#nullable enable
	public PartialFindResult<T> Find<T>(Func<Album, T> fac, AlbumQueryParams queryParams)
		where T : class
	{
		ParamIs.NotNull(() => queryParams);

		return HandleQuery(session =>
		{
			var result = Find(session, queryParams);

			return new PartialFindResult<T>(result.Items.Select(fac).ToArray(),
				result.TotalCount, result.Term);
		});
	}
#nullable disable

	public PartialFindResult<AlbumContract> Find(AlbumQueryParams queryParams)
	{
		return Find(s => new AlbumContract(s, LanguagePreference, PermissionContext), queryParams);
	}

	public PartialFindResult<AlbumContract> Find(
		SearchTextQuery textQuery, DiscType discType, int start, int maxResults, bool getTotalCount,
		AlbumSortRule sortRule = AlbumSortRule.Name, bool moveExactToTop = false)
	{
		var queryParams = new AlbumQueryParams(textQuery, discType, start, maxResults, getTotalCount, sortRule, moveExactToTop);
		return Find(queryParams);
	}

	public int? FindByMikuDbId(int mikuDbId)
	{
		return HandleQuery(session =>
		{
			var link = session.Query<AlbumWebLink>()
				.FirstOrDefault(w => !w.Entry.Deleted && w.Url.Contains("mikudb.com/" + mikuDbId + "/"));

			return (link != null ? (int?)link.Entry.Id : null);
		});
	}

	public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName)
	{
		var names = anyName.Where(n => !string.IsNullOrEmpty(n)).Select(n => n.Trim()).ToArray();

		if (!names.Any())
			return Array.Empty<EntryRefWithCommonPropertiesContract>();

		// TODO: moved Distinct after ToArray to work around NH bug
		return HandleQuery(session =>
		{
			return session.Query<AlbumName>()
				.Where(n => names.Contains(n.Value))
				.Select(n => n.Album)
				.Where(n => !n.Deleted)
				.Take(10)
				.ToArray()
				.Distinct()
				.Select(n => new EntryRefWithCommonPropertiesContract(n, PermissionContext.LanguagePreference))
				.ToArray();
		});
	}

	public string[] FindNames(SearchTextQuery textQuery, int maxResults)
	{
		if (textQuery.IsEmpty)
			return Array.Empty<string>();

		return HandleQuery(session =>
		{
			var names = session.Query<AlbumName>()
				.Where(a => !a.Album.Deleted)
				.WhereEntryNameIs(textQuery)
				.Select(n => n.Value)
				.OrderBy(n => n)
				.Distinct()
				.Take(maxResults)
				.ToArray();

			return NameHelper.MoveExactNamesToTop(names, textQuery.Query);
		});
	}

	public T GetAlbum<T>(int id, Func<Album, T> fac)
	{
		return HandleQuery(session => fac(session.Load<Album>(id)));
	}

	public AlbumContract GetAlbum(int id)
	{
		return GetAlbum(id, a => new AlbumContract(a, PermissionContext.LanguagePreference, PermissionContext));
	}

	public AlbumContract GetAlbumByLink(string link)
	{
		return HandleQuery(session =>
		{
			var webLink = session.Query<AlbumWebLink>().FirstOrDefault(p => p.Url.Contains(link));

			return (webLink != null ? new AlbumContract(webLink.Entry, PermissionContext.LanguagePreference, PermissionContext) : null);
		});
	}

	public string GetAlbumTagString(int id, string format, int? discNumber, bool includeHeader)
	{
		return GetAlbum(id, a => new AlbumSongFormatter(EntryLinkFactory)
			.ApplyFormat(a, format, discNumber, PermissionContext.LanguagePreference, includeHeader));
	}

	public AlbumContract GetAlbumWithAdditionalNames(int id)
	{
		return HandleQuery(session => new AlbumContract(
			session.Load<Album>(id),
			PermissionContext.LanguagePreference,
			PermissionContext
		));
	}

	[Obsolete]
	public AlbumWithArchivedVersionsContract GetAlbumWithArchivedVersions(int albumId)
	{
		return HandleQuery(session => new AlbumWithArchivedVersionsContract(
			session.Load<Album>(albumId),
			PermissionContext.LanguagePreference,
			PermissionContext,
			_userIconFactory
		));
	}

	public EntryForPictureDisplayContract GetArchivedAlbumPicture(int archivedVersionId)
	{
		return HandleQuery(session =>
			EntryForPictureDisplayContract.Create(
			session.Load<ArchivedAlbumVersion>(archivedVersionId), PermissionContext.LanguagePreference));
	}

	/// <summary>
	/// Gets the cover picture for a <see cref="Album"/>.
	/// </summary>
	/// <param name="id">Album Id.</param>
	/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
	/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
	public EntryForPictureDisplayContract GetCoverPicture(int id)
	{
		return HandleQuery(session =>
			EntryForPictureDisplayContract.Create(session.Load<Album>(id), PermissionContext.LanguagePreference));
	}

	public EntryWithTagUsagesContract GetEntryWithTagUsages(int albumId)
	{
		return HandleQuery(session =>
		{
			var album = session.Load<Album>(albumId);
			return new EntryWithTagUsagesContract(album, album.Tags.ActiveUsages, LanguagePreference, PermissionContext, _userIconFactory);
		});
	}

	public AlbumForUserContract[] GetUsersWithAlbumInCollection(int albumId)
	{
		return HandleQuery(session =>

			session.Load<Album>(albumId)
				.UserCollections
				.Where(a => a.PurchaseStatus != PurchaseStatus.Nothing)
				.OrderBy(u => u.User.Name)
				.Select(u => new AlbumForUserContract(
					u,
					LanguagePreference,
					PermissionContext,
					_userIconFactory,
					includeUser: u.User.Options.PublicAlbumCollection
				))
				.ToArray());
	}

	[Obsolete]
	public ArchivedAlbumVersionDetailsContract GetVersionDetails(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedAlbumVersionDetailsContract(session.Load<ArchivedAlbumVersion>(id),
				(comparedVersionId != 0 ? session.Load<ArchivedAlbumVersion>(comparedVersionId) : null), PermissionContext, _userIconFactory);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}

	public void Restore(int albumId)
	{
		PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

		HandleTransaction(session =>
		{
			var album = session.Load<Album>(albumId);

			album.Deleted = false;

			Archive(session, album, new AlbumDiff(false), AlbumArchiveReason.Restored);

			AuditLog("restored " + EntryLinkFactory.CreateEntryLink(album), session);
		});
	}

	public void UpdateArtistForAlbumIsSupport(int artistForAlbumId, bool isSupport)
	{
		VerifyManageDatabase();

		HandleTransaction(session =>
		{
			var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);
			var album = artistForAlbum.Album;

			artistForAlbum.IsSupport = isSupport;
			album.UpdateArtistString();

			AuditLog($"updated IsSupport for {artistForAlbum.ArtistToStringOrName} on {CreateEntryLink(album)}", session);

			session.Update(album);
		});
	}

	public void UpdateArtistForAlbumRoles(int artistForAlbumId, ArtistRoles roles)
	{
		VerifyManageDatabase();

		HandleTransaction(session =>
		{
			var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);
			var album = artistForAlbum.Album;

			artistForAlbum.Roles = roles;
			album.UpdateArtistString();

			AuditLog($"updated roles for {artistForAlbum.ArtistToStringOrName} on {CreateEntryLink(album)}", session);

			session.Update(album);
		});
	}
}

public enum AlbumSortRule
{
	None,

	Name,

	/// <summary>
	/// By release date in descending order, excluding entries without a full release date.
	/// </summary>
	ReleaseDate,

	/// <summary>
	/// By release date in descending order, including entries without a release date.
	/// Null release dates will be shown LAST (in descending order - in ascending order they'd be shown first).
	/// </summary>
	ReleaseDateWithNulls,

	AdditionDate,

	RatingAverage,

	RatingTotal,

	NameThenReleaseDate,

	CollectionCount
}
