using System;

namespace VocaDb.Model.Helpers {

	public static class DateTimeHelper {

		public static bool DateEquals(DateTime? first, DateTime? second) {
			
			if (!first.HasValue && !second.HasValue) // Both are null
				return true;

			if (!first.HasValue || !second.HasValue) // One is null
				return false;

			return first.Value.Date == second.Value.Date;

		}
			 
		public static DateTime? ParseDateTimeOffsetAsDate(string str) {
			
			DateTimeOffset date;
			if (DateTimeOffset.TryParse(str, out date)) {
				return date.Date;
			}

			return null;

		}

	}

}
