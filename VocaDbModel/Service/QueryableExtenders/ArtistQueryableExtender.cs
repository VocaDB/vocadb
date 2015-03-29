using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class ArtistQueryableExtender {

		public static IQueryable<ArtistName> WhereArtistNameIs(this IQueryable<ArtistName> query, ArtistSearchTextQuery textQuery) {

			var canonizedName = textQuery.Query;

			if (textQuery.IsExact) {

				return query.Where(m => m.Value == canonizedName
					|| m.Value == string.Format("{0}P", canonizedName)
					|| m.Value == string.Format("{0}-P", canonizedName));

			} else {

				return FindHelpers.AddEntryNameFilter(query, textQuery);

			}

		}

		public static IQueryable<ArtistName> WhereArtistTypeIn(this IQueryable<ArtistName> queryable, ArtistType[] types) {

			if (types == null || !types.Any())
				return queryable;

			return queryable.Where(n => types.Contains(n.Artist.ArtistType));

		}

		public static IQueryable<Artist> OrderBy(
			this IQueryable<Artist> criteria, ArtistSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case ArtistSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case ArtistSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case ArtistSortRule.AdditionDateAsc:
					return criteria.OrderBy(a => a.CreateDate);
				case ArtistSortRule.SongCount:
					return criteria.OrderByDescending(a => a.AllSongs.Count());
				case ArtistSortRule.SongRating:
					return criteria.OrderByDescending(a => a.AllSongs
						.Where(s => !s.Song.Deleted)
						.Sum(s => s.Song.RatingScore));
				case ArtistSortRule.FollowerCount:
					return criteria.OrderByDescending(a => a.Users.Count);
			}

			return criteria;

		}

		public static IQueryable<Artist> OrderBy(
			this IQueryable<Artist> query, EntrySortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case EntrySortRule.Name:
					return FindHelpers.AddNameOrder(query, languagePreference);
				case EntrySortRule.AdditionDate:
					return query.OrderByDescending(a => a.CreateDate);
			}

			return query;

		}

		public static IQueryable<Artist> WhereDraftsOnly(this IQueryable<Artist> query, bool draftsOnly) {

			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);

		}

		/// <summary>
		/// Filters an artist query by external link URL.
		/// </summary>
		/// <param name="query">Artists query. Cannot be null.</param>
		/// <param name="extLinkUrl">
		/// External link URL, for example http://www.nicovideo.jp/mylist/6667938.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Artist> WhereHasExternalLinkUrl(this IQueryable<Artist> query, string extLinkUrl) {
			
			if (string.IsNullOrEmpty(extLinkUrl))
				return query;

			return query.Where(a => a.WebLinks.Any(link => link.Url == extLinkUrl));

		} 

		// TODO: should be combined with common name query somehow, but NH is making it hard
		/// <summary>
		/// Filters an artist query by a name query.
		/// </summary>
		/// <param name="query">Artist query. Cannot be null.</param>
		/// <param name="textQuery">Textual filter. Cannot be null.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Artist> WhereHasName(this IQueryable<Artist> query, ArtistSearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => m.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					switch (words.Length) {
						case 1:
							query = query.Where(q => q.Names.Names.Any(n => n.Value.Contains(words[0])));
							break;
						case 2:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
							);
							break;
						case 3:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
							);
							break;
						case 4:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
							);
							break;
						case 5:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
							);
							break;
						case 6:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[5]))
							);
							break;
					}
					return query;

			}

			return query;

		}

		/// <summary>
		/// Filters query by canonized artist name.
		/// This means that any P suffixes are ignored.
		/// </summary>
		/// <param name="query">Artist query. Cannot be null.</param>
		/// <param name="textQuery">Textual filter. Cannot be null.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Artist> WhereHasName_Canonized(this IQueryable<Artist> query, ArtistSearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			var canonizedName = textQuery.Query;

			if (textQuery.IsExact) {

				return query.Where(m => m.Names.Names.Any(n => 
					n.Value == canonizedName
					|| n.Value == string.Format("{0}P", canonizedName)
					|| n.Value == string.Format("{0}-P", canonizedName)));

			} else {

				return query.WhereHasName(textQuery);

			}

		}

		public static IQueryable<Artist> WhereHasTag(this IQueryable<Artist> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Tags.Usages.Any(a => a.Tag.Name == tagName));

		}

		public static IQueryable<Artist> WhereHasType(this IQueryable<Artist> query, ArtistType[] artistTypes) {

			if (!artistTypes.Any())
				return query;

			return query.Where(m => artistTypes.Contains(m.ArtistType));

		}

		public static IQueryable<Artist> WhereIdIs(this IQueryable<Artist> query, int id) {
			
			if (id == 0)
				return query;

			return query.Where(m => m.Id == id);

		} 

		public static IQueryable<Artist> WhereIsFollowedByUser(this IQueryable<Artist> query, int userId) {

			if (userId == 0)
				return query;

			query = query.Where(s => s.Users.Any(a => a.User.Id == userId));

			return query;

		}

	}
}
