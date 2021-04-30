#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.Artists
{
	public class ArtistSearch
	{
		private readonly IEntryUrlParser _entryUrlParser;
		private readonly IDatabaseContext<Artist> _context;

		private ContentLanguagePreference LanguagePreference { get; }

		private IQueryable<Artist> CreateQuery(
			ArtistQueryParams queryParams,
			ParsedArtistQuery parsedQuery,
			NameMatchMode? nameMatchMode = null)
		{
			var textQuery = (parsedQuery.HasNameQuery ? queryParams.Common.TextQuery.OverrideMatchMode(nameMatchMode) : ArtistSearchTextQuery.Empty);

			var query = _context.Query()
				.Where(s => !s.Deleted)
				.WhereHasName_Canonized(textQuery)
				.WhereStatusIs(queryParams.Common.EntryStatus)
				.WhereHasExternalLinkUrl(parsedQuery.ExternalLinkUrl)
				.WhereHasType(queryParams.ArtistTypes)
				.WhereHasTags(queryParams.TagIds, queryParams.ChildTags)
				.WhereHasTags(queryParams.Tags)
				.WhereIdIs(parsedQuery.Id)
				.WhereIsFollowedByUser(queryParams.UserFollowerId)
				.WhereAllowBaseVoicebanks(queryParams.AllowBaseVoicebanks)
				.WhereMatchFilters(queryParams.AdvancedFilters);

			return query;
		}

		private ParsedArtistQuery FindInternalUrl(string trimmed, string trimmedLc)
		{
			if (trimmedLc.StartsWith("/ar/") || trimmedLc.StartsWith("http"))
			{
				var entryId = _entryUrlParser.Parse(trimmed, allowRelative: true);

				if (entryId.EntryType == EntryType.Artist)
					return new ParsedArtistQuery { Id = entryId.Id };
			}
			return null;
		}

		private ParsedArtistQuery FindExternalUrl(string trimmed, string trimmedLc)
		{
			if (trimmedLc.StartsWith("http") || trimmedLc.StartsWith("mylist/") || trimmedLc.StartsWith("user/") || trimmedLc.StartsWith("t/"))
			{
				var extUrl = new ArtistExternalUrlParser().GetExternalUrl(trimmed);

				if (extUrl != null)
					return new ParsedArtistQuery { ExternalLinkUrl = extUrl };
			}
			return null;
		}

		private ParsedArtistQuery ParseTextQuery(SearchTextQuery textQuery)
		{
			if (textQuery.IsEmpty)
				return new ParsedArtistQuery();

			var trimmed = textQuery.OriginalQuery.Trim();

			var term = SearchWord.GetTerm(trimmed, "id");

			if (term == null)
			{
				var trimmedLc = trimmed.ToLowerInvariant();

				// Optimization: check prefix, in most cases the user won't be searching by URL
				var result = FindInternalUrl(trimmed, trimmedLc);

				if (result != null)
					return result;

				result = FindExternalUrl(trimmed, trimmedLc);

				if (result != null)
					return result;
			}
			else
			{
				switch (term.PropertyName)
				{
					case "id":
						return new ParsedArtistQuery { Id = PrimitiveParseHelper.ParseIntOrDefault(term.Value, 0) };
				}
			}

			return new ParsedArtistQuery { Name = textQuery.Query };
		}

		private static Artist[] SortByIds(IEnumerable<Artist> songs, int[] idList)
		{
			return Model.Helpers.CollectionHelper.SortByIds(songs, idList);
		}

		public ArtistSearch(ContentLanguagePreference languagePreference, IDatabaseContext<Artist> context, IEntryUrlParser entryUrlParser)
		{
			LanguagePreference = languagePreference;
			_context = context;
			_entryUrlParser = entryUrlParser;
		}

		public PartialFindResult<Artist> Find(ArtistQueryParams queryParams)
		{
			var isMoveToTopQuery = (queryParams.Common.MoveExactToTop
				&& queryParams.Common.NameMatchMode != NameMatchMode.StartsWith
				&& !queryParams.Common.TextQuery.IsExact
				&& queryParams.Paging.Start == 0
				&& !queryParams.Common.TextQuery.IsEmpty);

			var parsedQuery = ParseTextQuery(queryParams.Common.TextQuery);

			if (isMoveToTopQuery)
			{
				return GetArtistsMoveExactToTop(queryParams, parsedQuery);
			}

			var query = CreateQuery(queryParams, parsedQuery);

			var ids = query
				.OrderBy(queryParams.SortRule, LanguagePreference)
				.Select(s => s.Id)
				.Paged(queryParams.Paging)
				.ToArray();

			var artists = SortByIds(_context
				.Query()
				.Where(s => ids.Contains(s.Id))
				.ToArray(), ids);

			var count = (queryParams.Paging.GetTotalCount ? query.Count() : 0);

			return new PartialFindResult<Artist>(artists, count, queryParams.Common.Query);
		}

		/// <summary>
		/// Get songs, searching by exact matches FIRST.
		/// This mode does not support paging.
		/// </summary>
		private PartialFindResult<Artist> GetArtistsMoveExactToTop(ArtistQueryParams queryParams, ParsedArtistQuery parsedQuery)
		{
			var sortRule = queryParams.SortRule;
			var maxResults = queryParams.Paging.MaxEntries;
			var getCount = queryParams.Paging.GetTotalCount;

			// Exact query contains the "exact" matches.
			// Note: the matched name does not have to be in user's display language, it can be any name.
			// The songs are sorted by user's display language though
			var exactQ = CreateQuery(queryParams, parsedQuery, NameMatchMode.StartsWith);

			int count;
			int[] ids;
			var exactResults = exactQ
				.OrderBy(sortRule, LanguagePreference)
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
					.OrderBy(sortRule, LanguagePreference)
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

			var artist = SortByIds(_context
				.Query()
				.Where(s => ids.Contains(s.Id))
				.ToArray(), ids);

			return new PartialFindResult<Artist>(artist, count, queryParams.Common.Query);
		}
	}
}
