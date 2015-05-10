using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class SongInListQueryableExtender {

		public static IQueryable<SongInList> OrderBy(this IQueryable<SongInList> query, SongSortRule? sortRule, ContentLanguagePreference languagePreference) {

			if (sortRule == null)
				return query.OrderBy(s => s.Order);

			return SongLinkQueryableExtender.OrderBy(query, sortRule.Value, languagePreference);

		}

	}

}
