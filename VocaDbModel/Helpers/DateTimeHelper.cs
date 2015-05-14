using System;

namespace VocaDb.Model.Helpers {

	public static class DateTimeHelper {

		public static DateTime? ParseDateTimeOffsetAsDate(string str) {
			
			DateTimeOffset date;
			if (DateTimeOffset.TryParse(str, out date)) {
				return date.Date;
			}

			return null;

		}

	}

}
