using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumSearch {

		private readonly ContentLanguagePreference languagePreference;
		private readonly IRepositoryContext querySource;

		private ContentLanguagePreference LanguagePreference {
			get { return languagePreference; }
		}

		private IQueryable<Album> CreateQuery(
			AlbumQueryParams queryParams, 
			ParsedAlbumQuery parsedQuery, 
			NameMatchMode? nameMatchMode = null) {
			
			var artistIds = EntryIdsCollection.CreateWithFallback(queryParams.ArtistIds, parsedQuery.ArtistId);
			var textQuery = SearchTextQuery.Create(parsedQuery.Name, nameMatchMode ?? queryParams.Common.NameMatchMode);

			var query = Query<Album>()
				.WhereIsDeleted(queryParams.Deleted)
				.WhereHasName(textQuery, allowCatNum: true)
				.WhereDraftsOnly(queryParams.Common.DraftOnly)
				.WhereStatusIs(queryParams.Common.EntryStatus)
				.WhereHasArtistParticipationStatus(artistIds, queryParams.ArtistParticipationStatus, queryParams.ChildVoicebanks, id => querySource.Load<Artist>(id))
				.WhereHasBarcode(queryParams.Barcode)
				.WhereHasType(queryParams.AlbumType)
				.WhereHasTags<Album, AlbumTagUsage>(queryParams.Tags != null && queryParams.Tags.Any() ? queryParams.Tags : new[] {  parsedQuery.TagName });

			return query;

		}

		private SearchWord GetTerm(string query, params string[] testTerms) {

			return (
				from term in testTerms 
				where query.StartsWith(term + ":", StringComparison.InvariantCultureIgnoreCase) 
				select new SearchWord(term, query.Substring(term.Length + 1).TrimStart()))
			.FirstOrDefault();

		}

		private ParsedAlbumQuery ParseTextQuery(string query) {
			
			if (string.IsNullOrWhiteSpace(query))
				return new ParsedAlbumQuery();

			var term = GetTerm(query.Trim(), "tag", "artist");
			
			if (term != null) {

				switch (term.PropertyName) {
					case "tag":
						return new ParsedAlbumQuery { TagName = term.Value };
					case "artist":
						int artistId;
						if (int.TryParse(term.Value, out artistId))
							return new ParsedAlbumQuery { ArtistId = int.Parse(term.Value) };						
						break;
				}
				
			}

			return new ParsedAlbumQuery { Name = query.Trim() };

		}

		public static Album[] SortByIds(IEnumerable<Album> albums, int[] idList) {
			
			return CollectionHelper.SortByIds(albums, idList);

		} 

		private PartialFindResult<Album> GetAlbums(AlbumQueryParams queryParams, ParsedAlbumQuery parsedQuery) {

			var query = CreateQuery(queryParams, parsedQuery);

			var ids = query
				.OrderBy(queryParams.SortRule, LanguagePreference)
				.Select(s => s.Id)
				.Paged(queryParams.Paging)
				.ToArray();

			var albums = SortByIds(querySource
				.Query<Album>()
				.Where(s => ids.Contains(s.Id))
				.ToArray(), ids);

			var count = (queryParams.Paging.GetTotalCount ? query.Count() : 0);

			return new PartialFindResult<Album>(albums, count, queryParams.Common.Query, false);

		}

		/// <summary>
		/// Get albums, searching by exact matches FIRST.
		/// This mode does not support paging.
		/// </summary>
		private PartialFindResult<Album> GetAlbumsMoveExactToTop(AlbumQueryParams queryParams, ParsedAlbumQuery parsedQuery) {
			
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

			var albums = SortByIds(
				querySource
					.Query<Album>()
					.Where(s => ids.Contains(s.Id))
					.ToArray(), ids);

			return new PartialFindResult<Album>(albums, count, queryParams.Common.Query, true);

		}

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
		}

		public AlbumSearch(IRepositoryContext querySource, ContentLanguagePreference languagePreference) {
			this.querySource = querySource;
			this.languagePreference = languagePreference;
		}

		public PartialFindResult<Album> Find(AlbumQueryParams queryParams) {

			var query = queryParams.Common.Query ?? string.Empty;
			var parsedQuery = ParseTextQuery(query);

			var isMoveToTopQuery = 	(queryParams.Common.MoveExactToTop 
				&& queryParams.Common.NameMatchMode != NameMatchMode.StartsWith 
				&& queryParams.Common.NameMatchMode != NameMatchMode.Exact 
				&& (queryParams.ArtistIds == null || !queryParams.ArtistIds.Any())
				&& queryParams.Paging.Start == 0
				&& parsedQuery.HasNameQuery);

			if (isMoveToTopQuery) {
				return GetAlbumsMoveExactToTop(queryParams, parsedQuery);
			}

			return GetAlbums(queryParams, parsedQuery);

		}

	}

}
