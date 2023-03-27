using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Database.Queries;

public class EntryQueries : QueriesBase<IAlbumRepository, Album>
{
	private readonly IAggregatedEntryImageUrlFactory _entryThumbPersister;

	public EntryQueries(IAlbumRepository repository, IUserPermissionContext permissionContext, IAggregatedEntryImageUrlFactory entryThumbPersister)
		: base(repository, permissionContext)
	{
		_entryThumbPersister = entryThumbPersister;
	}

	public PartialFindResult<EntryForApiContract> GetList(
		string query,
		int[]? tagIds,
		string[]? tags,
		bool childTags,
		EntryStatus? status,
		EntryTypes? entryTypes,
		int start, int maxResults, bool getTotalCount,
		EntrySortRule sort,
		NameMatchMode nameMatchMode,
		EntryOptionalFields fields,
		ContentLanguagePreference lang,
		bool searchTags = false,
		bool searchEvents = false
	)
	{
		var textQuery = SearchTextQuery.Create(query, nameMatchMode);
		var artistTextQuery = ArtistSearchTextQuery.Create(query, nameMatchMode); // Can't use the existing words collection here as they are noncanonized

		return _repository.HandleQuery(ctx =>
		{
			// Get all applicable names per entry type
			var artistQuery = ctx.OfType<Artist>().Query()
				.WhereEntryTypeIsIncluded(entryTypes, EntryType.Artist)
				.WhereNotDeleted()
				.WhereHasName_Canonized(artistTextQuery)
				.WhereHasTags(tagIds, childTags)
				.WhereHasTags(tags)
				.WhereStatusIs(status);

			var artistNames = artistQuery
				.OrderBy(sort, lang)
				.Take(start + maxResults)
				.SelectEntryBase(lang, EntryType.Artist)
				.ToArray();

			var albumQuery = ctx.OfType<Album>().Query()
				.WhereEntryTypeIsIncluded(entryTypes, EntryType.Album)
				.WhereNotDeleted()
				.WhereHasName(textQuery)
				.WhereHasTags(tagIds, childTags)
				.WhereHasTags(tags)
				.WhereStatusIs(status);

			var albumNames = albumQuery
				.OrderBy(sort, lang, null)
				.Take(start + maxResults)
				.SelectEntryBase(lang, EntryType.Album)
				.ToArray();

			var songQuery = ctx.OfType<Song>().Query()
				.WhereEntryTypeIsIncluded(entryTypes, EntryType.Song)
				.WhereNotDeleted()
				.WhereHasName(textQuery)
				.WhereHasTags(tagIds, childTags)
				.WhereHasTags(tags)
				.WhereStatusIs(status);

			var songNames = songQuery
				.OrderBy(sort, lang)
				.Take(start + maxResults)
				.SelectEntryBase(lang, EntryType.Song)
				.ToArray();

			var eventQuery = searchEvents ? ctx.OfType<ReleaseEvent>().Query()
				.WhereEntryTypeIsIncluded(entryTypes, EntryType.ReleaseEvent)
				.WhereNotDeleted()
				.WhereHasName(textQuery)
				.WhereHasTags(tagIds, childTags)
				.WhereStatusIs(status) : null;

			var eventNames = searchEvents ? eventQuery!
				.OrderBy(sort, lang, null)
				.Take(start + maxResults)
				.SelectEntryBase(lang, EntryType.ReleaseEvent)
				.ToArray() : null;

			var tagQuery = searchTags ? ctx.OfType<Tag>().Query()
				.WhereEntryTypeIsIncluded(entryTypes, EntryType.Tag)
				.WhereNotDeleted()
				.WhereHasName(textQuery)
				.WhereStatusIs(status) : null;

			var tagNames = searchTags ? tagQuery!
				.OrderBy(sort, lang)
				.Take(start + maxResults)
				.SelectEntryBase(lang, EntryType.Tag)
				.ToArray() : null;

			// Get page of combined names
			var entryNames = artistNames
				.Concat(albumNames)
				.Concat(songNames)
				.Concat(tagNames ?? Array.Empty<DataContracts.EntryBaseContract>())
				.Concat(eventNames ?? Array.Empty<DataContracts.EntryBaseContract>())
				.OrderBy(e => e.DefaultName)
				.Skip(start)
				.Take(maxResults)
				.ToArray();

			var artistIds = entryNames.Where(e => e.EntryType == EntryType.Artist).Select(a => a.Id).ToArray();
			var albumIds = entryNames.Where(e => e.EntryType == EntryType.Album).Select(a => a.Id).ToArray();
			var songIds = entryNames.Where(e => e.EntryType == EntryType.Song).Select(a => a.Id).ToArray();
			var searchedTagIds = searchTags ? entryNames.Where(e => e.EntryType == EntryType.Tag).Select(a => a.Id).ToArray() : Array.Empty<int>();
			var eventIds = searchEvents ? entryNames.Where(e => e.EntryType == EntryType.ReleaseEvent).Select(a => a.Id).ToArray() : Array.Empty<int>();

			// Get the actual entries in the page
			var artists = artistIds.Any() ? ctx.OfType<Artist>().Query()
				.Where(a => artistIds.Contains(a.Id))
				.ToArray()
				.Select(a => new EntryForApiContract(a, lang, PermissionContext, _entryThumbPersister, fields)) : Array.Empty<EntryForApiContract>();

			var albums = albumIds.Any() ? ctx.OfType<Album>().Query()
				.Where(a => albumIds.Contains(a.Id))
				.ToArray()
				.Select(a => new EntryForApiContract(a, lang, PermissionContext, _entryThumbPersister, fields)) : Array.Empty<EntryForApiContract>();

			var songs = songIds.Any() ? ctx.OfType<Song>().Query()
				.Where(a => songIds.Contains(a.Id))
				.ToArray()
				.Select(a => new EntryForApiContract(a, lang, fields)) : Array.Empty<EntryForApiContract>();

			var searchedTags = searchTags && searchedTagIds.Any() ? ctx.OfType<Tag>().Query()
				.Where(a => searchedTagIds.Contains(a.Id))
				.ToArray()
				.Select(a => new EntryForApiContract(a, lang, PermissionContext, _entryThumbPersister, fields)) : Array.Empty<EntryForApiContract>();

			var events = searchEvents && eventIds.Any() ? ctx.OfType<ReleaseEvent>().Query()
				.Where(a => eventIds.Contains(a.Id))
				.ToArray()
				.Select(a => new EntryForApiContract(a, lang, PermissionContext, _entryThumbPersister, fields)) : Array.Empty<EntryForApiContract>();

			// Merge and sort the final list
			var entries = artists.Concat(albums).Concat(songs).Concat(searchedTags).Concat(events);

			if (sort == EntrySortRule.Name)
				entries = entries.OrderBy(a => a.Name);

			if (sort == EntrySortRule.AdditionDate)
				entries = entries.OrderByDescending(a => a.CreateDate);

			if (sort == EntrySortRule.ActivityDate)
				entries = entries.OrderByDescending(a => a.ActivityDate);

			var count = 0;

			if (getTotalCount)
			{
				var artistCount =
					(artistNames.Length >= maxResults ? artistQuery.Count() : artistNames.Length);

				var albumCount =
					(albumNames.Length >= maxResults ? albumQuery.Count() : albumNames.Length);

				var songCount =
					(songNames.Length >= maxResults ? songQuery.Count() : songNames.Length);

				var tagCount =
					searchTags ? (tagNames!.Length >= maxResults ? tagQuery!.Count() : tagNames.Length) : 0;

				var eventCount =
					searchEvents ? (eventNames!.Length >= maxResults ? eventQuery!.Count() : eventNames.Length) : 0;

				count = artistCount + albumCount + songCount + tagCount + eventCount;
			}

			return new PartialFindResult<EntryForApiContract>(entries.ToArray(), count);
		});
	}
}
