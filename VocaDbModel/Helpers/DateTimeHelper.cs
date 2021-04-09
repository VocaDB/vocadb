using System;

namespace VocaDb.Model.Helpers
{
	public static class DateTimeHelper
	{
		public static bool DateEquals(DateTime? first, DateTime? second)
		{
			if (!first.HasValue && !second.HasValue) // Both are null
				return true;

			if (!first.HasValue || !second.HasValue) // One is null
				return false;

			return first.Value.Date == second.Value.Date;
		}

		public static string FormatMinSec(int seconds)
		{
			return FormatMinSec(TimeSpan.FromSeconds(seconds));
		}

		public static string FormatMinSec(TimeSpan timeSpan)
		{
			return $"{(int)timeSpan.TotalMinutes}:{(timeSpan.Seconds < 10 ? "0" : "")}{timeSpan.Seconds}";
		}

		public static DateTime? ParseDateTimeOffsetAsDate(string? str)
		{
			if (DateTimeOffset.TryParse(str, out DateTimeOffset date))
				return date.Date;

			return null;
		}
	}
}
