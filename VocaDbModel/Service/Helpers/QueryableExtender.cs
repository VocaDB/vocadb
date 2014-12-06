using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.Helpers {

	public static class QueryableExtender {

		public static IQueryable<T> AddEntryNameFilter<T>(this IQueryable<T> query, SearchTextQuery textQuery)
			where T : LocalizedString {

			return FindHelpers.AddEntryNameFilter(query, textQuery);

		}

		public static IOrderedQueryable<T> AddNameOrder<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference) 
			where T : IEntryWithNames {

			return FindHelpers.AddNameOrder(criteria, languagePreference);

		}

		public static IOrderedQueryable<T> AddSongNameOrder<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference)
			where T : ISongLink {

			switch (languagePreference) {
				case ContentLanguagePreference.Japanese:
					return criteria.OrderBy(e => e.Song.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return criteria.OrderBy(e => e.Song.Names.SortNames.English);
				default:
					return criteria.OrderBy(e => e.Song.Names.SortNames.Romaji);
			}

		}
	}

}
