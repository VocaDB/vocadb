using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class VenueQueryableExtender {

		public static IQueryable<Venue> OrderByName(this IQueryable<Venue> query, ContentLanguagePreference languagePreference) {
			return query.OrderByEntryName(languagePreference);
		}

		public static IQueryable<Venue> WhereHasName(this IQueryable<Venue> query, SearchTextQuery textQuery) {
			return query.WhereHasNameGeneric<Venue, VenueName>(textQuery);
		}

	}

}
