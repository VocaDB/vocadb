using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class LocalizedStringQueryableExtender {

		public static IQueryable<T> FilterByEntryName<T>(this IQueryable<T> query, SearchTextQuery textQuery)
			where T : LocalizedString {

			return FindHelpers.AddEntryNameFilter(query, textQuery);

		}

	}

}
