using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class ReleaseEventSeriesQueryableExtensions
	{
		public static IQueryable<ReleaseEventSeries> OrderByName(this IQueryable<ReleaseEventSeries> query, ContentLanguagePreference languagePreference)
		{
			return query.OrderByEntryName(languagePreference);
		}

		public static IQueryable<ReleaseEventSeries> WhereHasName(this IQueryable<ReleaseEventSeries> query, SearchTextQuery textQuery)
		{
			return query.WhereHasNameGeneric<ReleaseEventSeries, EventSeriesName>(textQuery);
		}
	}
}
