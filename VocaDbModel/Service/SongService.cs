#nullable disable

using NHibernate;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service;

public class SongService : ServiceBase
{
#nullable enable
	private readonly IEntryUrlParser _entryUrlParser;
	private readonly IUserIconFactory _userIconFactory;

	private PartialFindResult<Song> Find(ISession session, SongQueryParams queryParams)
	{
		return new SongSearch(
			new NHibernateDatabaseContext(session, PermissionContext),
			queryParams.LanguagePreference,
			_entryUrlParser
		).Find(queryParams);
	}

	public SongService(
		ISessionFactory sessionFactory,
		IUserPermissionContext permissionContext,
		IEntryLinkFactory entryLinkFactory,
		IEntryUrlParser entryUrlParser,
		IUserIconFactory userIconFactory
	)
		: base(sessionFactory, permissionContext, entryLinkFactory)
	{
		_entryUrlParser = entryUrlParser;
		_userIconFactory = userIconFactory;
	}

	public void AddSongToList(int listId, int songId, string notes)
	{
		PermissionContext.VerifyPermission(PermissionToken.EditProfile);

		HandleTransaction(session =>
		{
			var list = session.Load<SongList>(listId);
			var items = session.Query<SongInList>().Where(s => s.List.Id == listId);
			int order = 1;

			if (items.Any())
				order = items.Max(s => s.Order) + 1;

			EntryPermissionManager.VerifyEdit(PermissionContext, list);

			var song = session.Load<Song>(songId);

			var link = list.AddSong(song, order, notes);
			session.Save(link);

			SysLog($"added {song} to {list}");
		});
	}

	public void Archive(ISession session, Song song, SongDiff diff, SongArchiveReason reason, string notes = "")
	{
		var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
		var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
		session.Save(archived);
	}

	public void Archive(ISession session, Song song, SongArchiveReason reason, string notes = "")
	{
		Archive(session, song, new SongDiff(), reason, notes);
	}

	public void Delete(int id, string notes)
	{
		UpdateEntity<Song>(id, (session, song) =>
		{
			EntryPermissionManager.VerifyDelete(PermissionContext, song);

			AuditLog($"deleting song {EntryLinkFactory.CreateEntryLink(song)}", session);

			song.Delete();

			AddEntryEditedEntry(session, song, Domain.Activityfeed.EntryEditEvent.Deleted, null);

			Archive(session, song, new SongDiff(false), SongArchiveReason.Deleted, notes);
		}, PermissionToken.Nothing, skipLog: true);
	}
#nullable disable

	public T FindFirst<T>(Func<Song, ISession, T> fac, string[] query, NameMatchMode nameMatchMode)
		where T : class
	{
		return HandleQuery(session =>
		{
			foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q)))
			{
				var result = Find(session,
					new SongQueryParams
					{
						Common = new CommonSearchParams
						{
							TextQuery = SearchTextQuery.Create(q, nameMatchMode),
							OnlyByName = true,
							MoveExactToTop = true
						},
						LanguagePreference = LanguagePreference,
						Paging = new PagingProperties(0, 30, false)
					});

				if (result.Items.Any())
					return fac(result.Items.First(), session);
			}

			return null;
		});
	}

	public PartialFindResult<T> Find<T>(Func<Song, T> fac, SongQueryParams queryParams)
		where T : class
	{
		return HandleQuery(session =>
		{
			var result = Find(session, queryParams);

			return new PartialFindResult<T>(
				items: result.Items.Select(fac).ToArray(),
				totalCount: result.TotalCount,
				term: result.Term
			);
		});
	}

	public PartialFindResult<SongContract> Find(SongQueryParams queryParams)
	{
		return Find(s => new SongContract(s, PermissionContext.LanguagePreference), queryParams);
	}

	public PartialFindResult<SongWithAlbumAndPVsContract> FindWithAlbum(SongQueryParams queryParams, bool getPVs)
	{
		return Find(
			s => new SongWithAlbumAndPVsContract(
				s,
				PermissionContext.LanguagePreference,
				PermissionContext,
				getPVs
			),
			queryParams
		);
	}

	public PartialFindResult<SongContract> FindWithThumbPreferNotNico(SongQueryParams queryParams)
	{
		return Find(s => new SongContract(s, PermissionContext.LanguagePreference, VideoServiceHelper.GetThumbUrlPreferNotNico(s.PVs.PVs)), queryParams);
	}

	[Obsolete]
	public SongContract[] FindByName(string term, int maxResults)
	{
		return HandleQuery(session =>
		{
			var direct = session.Query<Song>()
				.Where(s =>
					!s.Deleted &&
					(string.IsNullOrEmpty(term)
						|| s.Names.SortNames.English.Contains(term)
						|| s.Names.SortNames.Romaji.Contains(term)
						|| s.Names.SortNames.Japanese.Contains(term)
					|| (s.NicoId != null && s.NicoId == term)))
				.Take(maxResults)
				.ToArray();

			var additionalNames = session.Query<SongName>()
				.Where(m => m.Value.Contains(term) && !m.Song.Deleted)
				.Select(m => m.Song)
				.Distinct()
				.Take(maxResults)
				.ToArray()
				.Where(a => !direct.Contains(a));

			return direct.Concat(additionalNames)
				.Take(maxResults)
				.Select(a => new SongContract(a, PermissionContext.LanguagePreference))
				.ToArray();
		});
	}

	public string[] FindNames(SearchTextQuery textQuery, int maxResults)
	{
		if (textQuery.IsEmpty)
			return Array.Empty<string>();

		return HandleQuery(session =>
		{
			var names = session.Query<SongName>()
				.Where(a => !a.Song.Deleted)
				.WhereEntryNameIs(textQuery)
				.Select(n => n.Value)
				.OrderBy(n => n)
				.Distinct()
				.Take(maxResults)
				.ToArray();

			return NameHelper.MoveExactNamesToTop(names, textQuery.Query);
		});
	}

	public EntryWithTagUsagesForApiContract GetEntryWithTagUsages(int songId)
	{
		return HandleQuery(session =>
		{
			var song = session.Load<Song>(songId);
			return new EntryWithTagUsagesForApiContract(song, song.Tags.ActiveUsages, LanguagePreference, PermissionContext, _userIconFactory);
		});
	}

	public T GetSong<T>(int id, Func<Song, T> fac)
	{
		return HandleQuery(session => fac(session.Load<Song>(id)));
	}

	public SongListBaseContract[] GetSongListsForCurrentUser(int ignoreSongId)
	{
		PermissionContext.VerifyLogin();

		var canEditPools = PermissionContext.HasPermission(PermissionToken.EditFeaturedLists);

		return HandleQuery(session =>
		{
			var ignoredLists = session
				.Query<SongInList>()
				.Where(sil => sil.Song.Id == ignoreSongId)
				.Select(sil => sil.List.Id)
				.Distinct()
				.ToArray();

			return session.Query<SongList>()
				.WhereNotDeleted()
				.Where(l => !ignoredLists.Contains(l.Id) &&
					((l.Author.Id == PermissionContext.LoggedUser.Id && l.FeaturedCategory == SongListFeaturedCategory.Nothing)
						|| (canEditPools && l.FeaturedCategory == SongListFeaturedCategory.Pools)))
				.OrderBy(l => l.Name)
				.ToArray()
				.Select(l => new SongListBaseContract(l))
				.ToArray();
		});
	}

	public SongContract GetSongWithAdditionalNames(int id)
	{
		return HandleQuery(session => new SongContract(session.Load<Song>(id), PermissionContext.LanguagePreference));
	}

	[Obsolete]
	public SongWithArchivedVersionsContract GetSongWithArchivedVersions(int songId)
	{
		return HandleQuery(session => new SongWithArchivedVersionsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference, _userIconFactory));
	}

#nullable enable
	public T? GetSongWithPV<T>(Func<Song, T> fac, PVService service, string pvId)
		where T : class
	{
		return HandleQuery(session =>
		{
			var pv = session.Query<PVForSong>()
				.FirstOrDefault(p => p.Service == service && p.PVId == pvId && !p.Song.Deleted);

			return (pv != null ? fac(pv.Song) : null);
		});
	}
#nullable disable

	public SongContract GetSongWithPV(PVService service, string pvId)
	{
		return GetSongWithPV(s => new SongContract(s, PermissionContext.LanguagePreference), service, pvId);
	}

	public SongWithAlbumContract GetSongWithPVAndAlbum(PVService service, string pvId)
	{
		return GetSongWithPV(
			s => new SongWithAlbumContract(
				s, PermissionContext.LanguagePreference,
				PermissionContext
			),
			service,
			pvId
		);
	}

	public SongContract[] GetSongs(string filter, int start, int count)
	{
		return HandleQuery(session => session.Query<Song>()
			.Where(s => string.IsNullOrEmpty(filter)
				|| s.Names.SortNames.Japanese.Contains(filter)
				|| s.NicoId == filter)
			.OrderBy(s => s.Names.SortNames.Japanese)
			.Skip(start)
			.Take(count)
			.ToArray()
			.Select(s => new SongContract(s, PermissionContext.LanguagePreference))
			.ToArray());
	}

	[Obsolete]
	public ArchivedSongVersionDetailsContract GetVersionDetails(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedSongVersionDetailsContract(session.Load<ArchivedSongVersion>(id),
				comparedVersionId != 0 ? session.Load<ArchivedSongVersion>(comparedVersionId) : null,
				PermissionContext, _userIconFactory);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}

	public void Restore(int songId)
	{
		PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

		HandleTransaction(session =>
		{
			var song = session.Load<Song>(songId);

			song.Deleted = false;

			Archive(session, song, new SongDiff(false), SongArchiveReason.Restored);

			AuditLog("restored " + EntryLinkFactory.CreateEntryLink(song), session);
		});
	}
}

public enum SongSortRule
{
	None,
	Name,
	AdditionDate,
	PublishDate,
	FavoritedTimes,
	RatingScore,
	TagUsageCount,
	SongType,
}
