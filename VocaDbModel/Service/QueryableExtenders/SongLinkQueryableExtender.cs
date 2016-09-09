using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	/// <summary>
	/// Query extension methods for <see cref="ISongLink"/>.
	/// </summary>
	public static class SongLinkQueryableExtender {

		public static IOrderedQueryable<T> OrderBySongName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference)
			where T : ISongLink {

			switch (languagePreference) {
				case ContentLanguagePreference.Japanese:
					return query.OrderBy(e => e.Song.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return query.OrderBy(e => e.Song.Names.SortNames.English);
				default:
					return query.OrderBy(e => e.Song.Names.SortNames.Romaji);
			}

		}

		public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, SongSortRule sortRule, ContentLanguagePreference languagePreference)
			where T : ISongLink {

			switch (sortRule) {
				case SongSortRule.Name:
					return query.OrderBySongName(languagePreference);
				case SongSortRule.AdditionDate:
					return query.OrderByDescending(a => a.Song.CreateDate);
				case SongSortRule.FavoritedTimes:
					return query.OrderByDescending(a => a.Song.FavoritedTimes);
				case SongSortRule.RatingScore:
					return query.OrderByDescending(a => a.Song.RatingScore);
				case SongSortRule.PublishDate:
					return query.OrderByDescending(a => a.Song.PublishDate.DateTime);
			}

			return query;

		}

		public static IQueryable<T> WhereSongHasArtist<T>(this IQueryable<T> query, int artistId, bool childVoicebanks)
			where T : ISongLink {
			
			if (artistId == 0)
				return query;

			if (!childVoicebanks)
				return query.Where(s => s.Song.AllArtists.Any(a => a.Artist.Id == artistId));
			else
				return query.Where(s => s.Song.AllArtists.Any(a => a.Artist.Id == artistId || a.Artist.BaseVoicebank.Id == artistId));

		}

		public static IQueryable<T> WhereSongHasArtists<T>(this IQueryable<T> query, EntryIdsCollection artistIds, bool childVoicebanks)
			where T : ISongLink {

			if (!artistIds.HasAny)
				return query;

			return artistIds.Ids.Aggregate(query, (current, artistId) => current.WhereSongHasArtist(artistId, childVoicebanks));

		}

		public static IQueryable<T> WhereSongHasArtistWithType<T>(this IQueryable<T> query, ArtistType artistType)
			where T : ISongLink {

			if (artistType == ArtistType.Unknown)
				return query;

			return query.Where(s => s.Song.AllArtists.Any(a => !a.IsSupport && a.Artist.ArtistType == artistType));

		}

		public static IQueryable<T> WhereSongHasLyrics<T>(this IQueryable<T> query, ContentLanguageSelection[] languages)
			where T : ISongLink {

			// TODO
			return query;

			/*if (languages == null || !languages.Any())
				return query;

			if (languages.Length == 1) {
				var lang = languages.First();
				return query.Where(s => s.Song.Lyrics.Any(l => l.Language == lang));
			} else {

				var allLanguages = EnumVal<ContentLanguageSelection>.Values.All(languages.Contains);

				if (allLanguages) {
					// Has lyrics in any language
					return query.Where(s => s.Song.Lyrics.Any());
				} else {
					return query.Where(s => s.Song.Lyrics.Any(l => languages.Contains(l.Language)));
				}

			}*/

		}
		public static IQueryable<T> WhereSongIsInList<T>(this IQueryable<T> query, int listId)
			where T : ISongLink {
			
			if (listId == 0)
				return query;

			return query.Where(s => s.Song.ListLinks.Any(l => l.List.Id == listId));

		}

		/// <summary>
		/// Filter query by PV services bit array.
		/// Song will pass the filter if ANY of the specified PV services matches.
		/// </summary>
		/// <typeparam name="T">Type of song link.</typeparam>
		/// <param name="query">Query. Cannot be null.</param>
		/// <param name="pvServices">PV services bit array. Can be null, in which case no filtering will be done.</param>
		/// <returns>Filtered query.</returns>
		public static IQueryable<T> WhereSongHasPVService<T>(this IQueryable<T> query, PVServices? pvServices)
			where T : ISongLink {
			
			if (pvServices == null)
				return query;

			return query.Where(s => (s.Song.PVServices & pvServices) != PVServices.Nothing);

		} 

		public static IQueryable<T> WhereSongHasTag<T>(this IQueryable<T> query, string tagName)
			where T : ISongLink {
			
			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Song.Tags.Usages.Any(t => t.Tag.Names.SortNames.English == tagName || t.Tag.Names.SortNames.Romaji == tagName || t.Tag.Names.SortNames.Japanese == tagName));

		}

		public static IQueryable<T> WhereSongHasTag<T>(this IQueryable<T> query, int tagId)
			where T : ISongLink {

			if (tagId == 0)
				return query;

			return query.Where(s => s.Song.Tags.Usages.Any(t => t.Tag.Id == tagId));

		}

		public static IQueryable<T> WhereSongHasTags<T>(this IQueryable<T> query, int[] tagIds)
			where T : ISongLink {

			if (tagIds == null || !tagIds.Any())
				return query;

			return tagIds.Aggregate(query, WhereSongHasTag);

		}

		public static IQueryable<T> WhereSongHasType<T>(this IQueryable<T> query, SongType[] songTypes)
			where T: ISongLink {

			if (songTypes == null || !songTypes.Any())
				return query;

			return query.Where(m => songTypes.Contains(m.Song.SongType));

		}

		public static IQueryable<T> WhereMatchFilter<T>(this IQueryable<T> query, AdvancedSearchFilter filter)
			where T : ISongLink {

			if (filter == null)
				return query;

			switch (filter.FilterType) {
				case AdvancedFilterType.ArtistType: {
					var param = EnumVal<ArtistType>.Parse(filter.Param);
					return WhereSongHasArtistWithType(query, param);
				}
				case AdvancedFilterType.HasMultipleVoicebanks: {
						return query.Where(s => s.Song.AllArtists.Count(a => !a.IsSupport && ArtistHelper.VoiceSynthesizerTypes.Contains(a.Artist.ArtistType)) > 1);
					}
				case AdvancedFilterType.Lyrics: {
					var param = EnumVal<ContentLanguageSelection>.ParseMultiple(filter.Param);
					return WhereSongHasLyrics(query, param);
				}
			}

			return query;

		}

		public static IQueryable<T> WhereMatchFilters<T>(this IQueryable<T> query, IEnumerable<AdvancedSearchFilter> filters)
			where T : ISongLink {

			return filters?.Aggregate(query, WhereMatchFilter) ?? query;

		}

	}

}
