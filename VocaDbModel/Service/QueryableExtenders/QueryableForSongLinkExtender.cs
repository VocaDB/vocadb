using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.QueryableExtenders {

	/// <summary>
	/// Query extension methods for <see cref="ISongLink"/>.
	/// </summary>
	public static class QueryableForSongLinkExtender {

		public static IQueryable<T> AddSongOrder<T>(this IQueryable<T> criteria, SongSortRule sortRule, ContentLanguagePreference languagePreference)
			where T : ISongLink {

			switch (sortRule) {
				case SongSortRule.Name:
					return criteria.AddSongNameOrder(languagePreference);
				case SongSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.Song.CreateDate);
				case SongSortRule.FavoritedTimes:
					return criteria.OrderByDescending(a => a.Song.FavoritedTimes);
				case SongSortRule.RatingScore:
					return criteria.OrderByDescending(a => a.Song.RatingScore);
			}

			return criteria;

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

		public static IQueryable<T> WhereSongHasTag<T>(this IQueryable<T> query, string tag)
			where T : ISongLink {
			
			if (string.IsNullOrEmpty(tag))
				return query;

			return query.Where(s => s.Song.Tags.Usages.Any(t => t.Tag.Name == tag));

		}

	}

}
