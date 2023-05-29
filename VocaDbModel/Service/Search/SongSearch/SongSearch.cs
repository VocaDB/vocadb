#nullable disable

using System.Globalization;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.Search.SongSearch;

public class SongSearch
{
#nullable enable
	private readonly IEntryUrlParser _entryUrlParser;
	private readonly ContentLanguagePreference _languagePreference;
	private readonly IDatabaseContext _querySource;

	private ContentLanguagePreference LanguagePreference => _languagePreference;
#nullable disable

	private IQueryable<Song> CreateQuery(
		SongQueryParams queryParams,
		ParsedSongQuery parsedQuery,
		NameMatchMode? nameMatchMode = null
	)
	{
		var textQuery = !SearchTextQuery.IsNullOrEmpty(parsedQuery.Name)
			? new SearchTextQuery(
				query: parsedQuery.Name.Query,
				matchMode: nameMatchMode ?? parsedQuery.Name.MatchMode,
				originalQuery: parsedQuery.Name.OriginalQuery
			)
			: SearchTextQuery.Empty;

		textQuery = ProcessAdvancedSearch(textQuery, queryParams);

		var typesAndTags = ProcessUnifiedTypesAndTags(ref queryParams);

		var query = Query<Song>()
			.WhereNotDeleted()
			.WhereHasName(textQuery)
			.WhereHasArtistParticipationStatus(queryParams.ArtistParticipation, _querySource.OfType<Artist>())
			.WhereHasArtists<Song, ArtistForSong>(queryParams.ArtistNames)
			.WhereStatusIs(queryParams.Common.EntryStatus)
			.WhereHasType(queryParams.SongTypes)
			.WhereHasTags(queryParams.TagIds, queryParams.ChildTags)
			.WhereHasTags(queryParams.Tags)
			.WhereHasTag(parsedQuery.TagName)
			.WhereHasTypeOrTag(typesAndTags)
			.WhereArtistHasTag(parsedQuery.ArtistTag)
			.WhereArtistHasType(parsedQuery.ArtistType)
			.WhereHasNicoId(parsedQuery.NicoId)
			.WhereHasPV(parsedQuery.PV)
			.WhereHasPVService(queryParams.PVServices)
			.WhereIdIs(parsedQuery.Id)
			.WhereIdNotIn(queryParams.IgnoredIds)
			.WhereInUserCollection(queryParams.UserCollectionId)
			.WhereHasParentSong(queryParams.ParentSongId)
			.WhereArtistIsFollowedByUser(queryParams.FollowedByUserId)
			.WhereReleaseEventIs(queryParams.ReleaseEventId)
			.WherePublishDateIsBetween(parsedQuery.PublishedAfter, parsedQuery.PublishedBefore)
			.WherePublishDateIsBetween(queryParams.AfterDate, queryParams.BeforeDate)
			.WhereHasScore(queryParams.MinScore)
			.WhereCreateDateIsWithin(queryParams.TimeFilter)
			.WhereHasPV(queryParams.OnlyWithPVs)
			.WhereMatchFilters(queryParams.AdvancedFilters)
			.WhereMilliBpmIsBetween(queryParams.MinMilliBpm, queryParams.MaxMilliBpm)
			.WhereLengthIsBetween(queryParams.MinLength, queryParams.MaxLength)
			.WhereHasLanguage(queryParams.Language);

		return query;
	}

	private SearchWord GetTerm(string query, params string[] testTerms)
	{
		return SearchWord.GetTerm(query, testTerms);
	}

	private EntryTypeAndTagCollection<SongType> ProcessUnifiedTypesAndTags(ref SongQueryParams queryParams)
	{
		EntryTypeAndTagCollection<SongType> typesAndTags = null;

		if (queryParams.UnifyEntryTypesAndTags)
		{
			typesAndTags = EntryTypeAndTagCollection<SongType>.Create(EntryType.Song, queryParams.SongTypes, queryParams.TagIds, _querySource);
			queryParams = queryParams with
			{
				TagIds = queryParams.TagIds.Except(typesAndTags.TagIds).ToArray(),
				SongTypes = queryParams.SongTypes.Except(typesAndTags.SubTypes).ToArray(),
			};
		}

		return typesAndTags;
	}

	private SearchTextQuery ProcessAdvancedSearch(SearchTextQuery textQuery, SongQueryParams queryParams)
	{
		if (textQuery.IsEmpty || textQuery.MatchMode == NameMatchMode.Exact || textQuery.MatchMode == NameMatchMode.StartsWith || !textQuery.OriginalQuery.StartsWith("!"))
			return textQuery;

		var parsed = SearchParser.ParseQuery(textQuery.OriginalQuery.Substring(1));

		var artistNames = parsed.GetValues("artist").ToArray();

		if (artistNames.Any())
		{
			queryParams = queryParams with { ArtistNames = artistNames };
		}

		var words = parsed.GetValues("").ToArray();

		if (words.Any())
		{
			queryParams = queryParams with { Common = queryParams.Common with { TextQuery = new SearchTextQuery(textQuery.Query, NameMatchMode.Words, textQuery.OriginalQuery, words) } };
			return queryParams.Common.TextQuery;
		}
		else
		{
			return textQuery;
		}
	}

	public static Song[] SortByIds(IEnumerable<Song> songs, int[] idList)
	{
		return CollectionHelper.SortByIds(songs, idList);
	}

	private IQueryable<T> Query<T>() where T : class, IDatabaseObject
	{
		return _querySource.Query<T>();
	}

	private DateTime? ParseDateOrNull(string str) => DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed) ? parsed : null;

	private ParsedSongQuery ParseDateRange(string str)
	{
		if (string.IsNullOrEmpty(str))
			return new ParsedSongQuery();

		var parts = str.Split('-');

		if (parts.Length == 0)
			return new ParsedSongQuery();

		return new ParsedSongQuery
		{
			PublishedAfter = ParseDateOrNull(parts[0]),
			PublishedBefore = parts.Length > 1 ? ParseDateOrNull(parts[1]) : null
		};
	}

	private ParsedSongQuery ParseReferenceQuery(string trimmed, string query)
	{
		// Optimization: check prefix, in most cases the user won't be searching by URL
		if (trimmed.StartsWith("/s/", StringComparison.InvariantCultureIgnoreCase))
		{
			var entryId = _entryUrlParser.Parse(trimmed, allowRelative: true);

			if (entryId.EntryType == EntryType.Song)
				return new ParsedSongQuery { Id = entryId.Id };
		}
		else if (trimmed.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
		{
			// Test PV URL with services that don't require a web call
			var videoParseResult = VideoServiceHelper.ParseByUrlAsync(query, false, null,
				VideoService.NicoNicoDouga, VideoService.Youtube, VideoService.Bilibili, VideoService.File, VideoService.LocalFile, VideoService.Vimeo).Result;

			if (videoParseResult.IsOk)
			{
				if (videoParseResult.Service == Domain.PVs.PVService.NicoNicoDouga)
				{
					return new ParsedSongQuery { NicoId = videoParseResult.Id };
				}
				else
				{
					return new ParsedSongQuery { PV = new PVContract { PVId = videoParseResult.Id, Service = videoParseResult.Service } };
				}
			}

			var entryId = _entryUrlParser.Parse(trimmed, allowRelative: false);

			if (entryId.EntryType == EntryType.Song)
				return new ParsedSongQuery { Id = entryId.Id };
		}

		return null;
	}

	public ParsedSongQuery ParseTextQuery(SearchTextQuery textQuery)
	{
		var query = textQuery.OriginalQuery;

		if (string.IsNullOrWhiteSpace(query))
			return new ParsedSongQuery();

		var trimmed = query.Trim();

		var term = GetTerm(trimmed, "id", "tag", "artist-tag", "artist-type", "publish-date");

		return (term?.PropertyName) switch
		{
			"tag" => new ParsedSongQuery { TagName = term.Value },
			"artist-tag" => new ParsedSongQuery { ArtistTag = term.Value },
			"artist-type" => new ParsedSongQuery { ArtistType = EnumVal<ArtistType>.ParseSafe(term.Value, ArtistType.Unknown) },
			"id" => new ParsedSongQuery { Id = PrimitiveParseHelper.ParseIntOrDefault(term.Value, 0) },
			"publish-date" => ParseDateRange(term.Value),
			_ => ParseReferenceQuery(trimmed, query) ?? new ParsedSongQuery { Name = textQuery },
		};
	}

#nullable enable
	public SongSearch(IDatabaseContext querySource, ContentLanguagePreference languagePreference, IEntryUrlParser entryUrlParser)
	{
		_querySource = querySource;
		_languagePreference = languagePreference;
		_entryUrlParser = entryUrlParser;
	}

	/// <summary>
	/// Finds songs based on criteria.
	/// </summary>
	/// <param name="queryParams">Query parameters. Cannot be null.</param>
	/// <returns>List of song search results. Cannot be null.</returns>
	public PartialFindResult<Song> Find(SongQueryParams queryParams)
	{
		ParamIs.NotNull(() => queryParams);

		var parsedQuery = ParseTextQuery(queryParams.Common.TextQuery);

		var isMoveToTopQuery =
			queryParams.Common.MoveExactToTop &&
			queryParams.Common.NameMatchMode != NameMatchMode.StartsWith &&
			queryParams.Common.NameMatchMode != NameMatchMode.Exact &&
			!queryParams.ArtistParticipation.ArtistIds.HasAny &&
			queryParams.Paging.Start == 0 &&
			parsedQuery.HasNameQuery;

		if (isMoveToTopQuery)
		{
			return GetSongsMoveExactToTop(queryParams, parsedQuery);
		}

		return GetSongs(queryParams, parsedQuery);
	}

	/// <summary>
	/// Get songs, searching by exact matches FIRST.
	/// This mode does not support paging.
	/// </summary>
	private PartialFindResult<Song> GetSongsMoveExactToTop(SongQueryParams queryParams, ParsedSongQuery parsedQuery)
	{
		var sortRule = queryParams.SortRule;
		var maxResults = queryParams.Paging.MaxEntries;
		var getCount = queryParams.Paging.GetTotalCount;

		// Exact query contains the "exact" matches.
		// Note: the matched name does not have to be in user's display language, it can be any name.
		// The songs are sorted by user's display language though
		var exactQ = CreateQuery(queryParams, parsedQuery, NameMatchMode.Exact);

		int count;
		int[] ids;
		var exactResults = exactQ
			.OrderBy(sortRule, LanguagePreference, queryParams.TagIds?.FirstOrDefault() ?? 0)
			.Select(s => s.Id)
			.Take(maxResults)
			.ToArray();

		if (exactResults.Length >= maxResults)
		{
			ids = exactResults;
			count = getCount ? CreateQuery(queryParams, parsedQuery).Count() : 0;
		}
		else
		{
			var directQ = CreateQuery(queryParams, parsedQuery);

			var direct = directQ
				.OrderBy(sortRule, LanguagePreference, queryParams.TagIds?.FirstOrDefault() ?? 0)
				.Select(s => s.Id)
				.Take(maxResults)
				.ToArray();

			ids = exactResults
				.Concat(direct)
				.Distinct()
				.Take(maxResults)
				.ToArray();

			count = getCount ? directQ.Count() : 0;
		}

		var songs = _querySource
			.LoadMultiple<Song>(ids)
			.ToArray()
			.OrderByIds(ids);

		return new PartialFindResult<Song>(songs, count, queryParams.Common.Query);
	}
#nullable disable

	private PartialFindResult<Song> GetSongs(SongQueryParams queryParams, ParsedSongQuery parsedQuery)
	{
		var query = CreateQuery(queryParams, parsedQuery);

		var ids = query
			.OrderBy(queryParams.SortRule, LanguagePreference, queryParams.TagIds?.FirstOrDefault() ?? 0)
			.Select(s => s.Id)
			.Paged(queryParams.Paging)
			.ToArray();

		var songs = SortByIds(
			songs: _querySource
				.Query<Song>()
				.Where(s => ids.Contains(s.Id))
				.ToArray(),
			idList: ids
		);

		var count = queryParams.Paging.GetTotalCount ? query.Count() : 0;

		return new PartialFindResult<Song>(
			items: songs,
			totalCount: count,
			term: queryParams.Common.Query
		);
	}
}
