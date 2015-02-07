using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search.SongSearch;

namespace VocaDb.Model.Service.Search.Artists {

	public class ArtistSearch {

		private readonly IEntryUrlParser entryUrlParser;
		private readonly ContentLanguagePreference languagePreference;
		private readonly IRepositoryContext<Artist> context; 

		private ContentLanguagePreference LanguagePreference {
			get { return languagePreference; }
		}

		private IQueryable<Artist> CreateQuery(
			ArtistQueryParams queryParams, 
			ParsedArtistQuery parsedQuery,
			NameMatchMode? nameMatchMode = null) {
			
			var textQuery = (parsedQuery.HasNameQuery ? queryParams.Common.TextQuery.OverrideMatchMode(nameMatchMode) : ArtistSearchTextQuery.Empty);

			var query = context.Query()
				.Where(s => !s.Deleted)
				.WhereHasName_Canonized(textQuery)
				.WhereDraftsOnly(queryParams.Common.DraftOnly)
				.WhereStatusIs(queryParams.Common.EntryStatus)
				.WhereHasType(queryParams.ArtistTypes)
				.WhereHasTag(queryParams.Tag)
				.WhereIdIs(parsedQuery.Id)
				.WhereIsFollowedByUser(queryParams.UserFollowerId);

			return query;

		}

		private ParsedArtistQuery ParseTextQuery(string query) {
			
			if (string.IsNullOrWhiteSpace(query))
				return new ParsedArtistQuery();

			var trimmed = query.Trim();

			var term = SearchWord.GetTerm(trimmed, "id");
			
			if (term == null) {

				// Optimization: check prefix, in most cases the user won't be searching by URL
				if (trimmed.StartsWith("/ar/", StringComparison.InvariantCultureIgnoreCase) 
					|| trimmed.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)) {

					var entryId = entryUrlParser.Parse(trimmed, allowRelative: true);

					if (entryId.EntryType == EntryType.Artist)
						return new ParsedArtistQuery { Id = entryId.Id };
					
				}

			} else {

				switch (term.PropertyName) {
					case "id":
						return new ParsedArtistQuery { Id = PrimitiveParseHelper.ParseIntOrDefault(term.Value, 0) };
				}
				
			}

			return new ParsedArtistQuery { Name = trimmed };

		}

		private static Artist[] SortByIds(IEnumerable<Artist> songs, int[] idList) {
			
			return Model.Helpers.CollectionHelper.SortByIds(songs, idList);

		} 

		public ArtistSearch(ContentLanguagePreference languagePreference, IRepositoryContext<Artist> context, IEntryUrlParser entryUrlParser) {
			this.languagePreference = languagePreference;
			this.context = context;
			this.entryUrlParser = entryUrlParser;
		}

		public PartialFindResult<Artist> Find(ArtistQueryParams queryParams) {

			var isMoveToTopQuery = (queryParams.Common.MoveExactToTop 
				&& queryParams.Common.NameMatchMode != NameMatchMode.StartsWith 
				&& !queryParams.Common.TextQuery.IsExact
				&& queryParams.Paging.Start == 0
				&& !queryParams.Common.TextQuery.IsEmpty);

			var parsedQuery = ParseTextQuery(queryParams.Common.Query);

			if (isMoveToTopQuery) {
				return GetArtistsMoveExactToTop(queryParams, parsedQuery);
			}

			var query = CreateQuery(queryParams, parsedQuery);

			var ids = query
				.OrderBy(queryParams.SortRule, LanguagePreference)
				.Select(s => s.Id)
				.Paged(queryParams.Paging)
				.ToArray();

			var artists = SortByIds(context
				.Query()
				.Where(s => ids.Contains(s.Id))
				.ToArray(), ids);

			var count = (queryParams.Paging.GetTotalCount ? query.Count() : 0);

			return new PartialFindResult<Artist>(artists, count, queryParams.Common.Query, false);

		}

		/// <summary>
		/// Get songs, searching by exact matches FIRST.
		/// This mode does not support paging.
		/// </summary>
		private PartialFindResult<Artist> GetArtistsMoveExactToTop(ArtistQueryParams queryParams, ParsedArtistQuery parsedQuery) {
			
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

			if (exactResults.Length >= maxResults) {

				ids = exactResults;
				count = getCount ? CreateQuery(queryParams, parsedQuery).Count() : 0;

			} else { 

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

			var artist = SortByIds(context
				.Query()
				.Where(s => ids.Contains(s.Id))
				.ToArray(), ids);

			return new PartialFindResult<Artist>(artist, count, queryParams.Common.Query, true);

		}

	}
}
