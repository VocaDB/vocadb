using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Search.Artists {

	public class ArtistSearch {

		private readonly ContentLanguagePreference languagePreference;
		private readonly IRepositoryContext<Artist> context; 

		private ContentLanguagePreference LanguagePreference {
			get { return languagePreference; }
		}

		private IQueryable<Artist> CreateQuery(
			ArtistQueryParams queryParams, 
			NameMatchMode? nameMatchMode = null) {
			
			var query = context.Query()
				.Where(s => !s.Deleted)
				.WhereHasName_Canonized(queryParams.Common.Query, matchMode: nameMatchMode ?? queryParams.Common.NameMatchMode)
				.WhereDraftsOnly(queryParams.Common.DraftOnly)
				.WhereStatusIs(queryParams.Common.EntryStatus)
				.WhereHasType(queryParams.ArtistTypes)
				.WhereHasTag(queryParams.Tag)
				.WhereIsFollowedByUser(queryParams.UserFollowerId);

			return query;

		}

		private static Artist[] SortByIds(IEnumerable<Artist> songs, int[] idList) {
			
			return Model.Helpers.CollectionHelper.SortByIds(songs, idList);

		} 

		public ArtistSearch(ContentLanguagePreference languagePreference, IRepositoryContext<Artist> context) {
			this.languagePreference = languagePreference;
			this.context = context;
		}

		public PartialFindResult<Artist> Find(ArtistQueryParams queryParams) {

			var isMoveToTopQuery = 	(queryParams.Common.MoveExactToTop 
				&& queryParams.Common.NameMatchMode != NameMatchMode.StartsWith 
				&& queryParams.Common.NameMatchMode != NameMatchMode.Exact 
				&& queryParams.Paging.Start == 0
				&& !string.IsNullOrEmpty(queryParams.Common.Query));

			if (isMoveToTopQuery) {
				return GetArtistsMoveExactToTop(queryParams);
			}

			var query = CreateQuery(queryParams);

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
		private PartialFindResult<Artist> GetArtistsMoveExactToTop(ArtistQueryParams queryParams) {
			
			var sortRule = queryParams.SortRule;
			var maxResults = queryParams.Paging.MaxEntries;
			var getCount = queryParams.Paging.GetTotalCount;

			// Exact query contains the "exact" matches.
			// Note: the matched name does not have to be in user's display language, it can be any name.
			// The songs are sorted by user's display language though
			var exactQ = CreateQuery(queryParams, NameMatchMode.StartsWith);

			int count;
			int[] ids;
			var exactResults = exactQ
				.OrderBy(sortRule, LanguagePreference)
				.Select(s => s.Id)
				.Take(maxResults)
				.ToArray();

			if (exactResults.Length >= maxResults) {

				ids = exactResults;
				count = getCount ? CreateQuery(queryParams).Count() : 0;

			} else { 

				var directQ = CreateQuery(queryParams);

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
