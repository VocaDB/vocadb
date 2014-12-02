using System;
using VocaDb.Web.Resources.Views.Shared;

namespace VocaDb.Web.Code {

	public static class TimeAgoStringBuilder {

		public static string FormatTimeAgo(TimeSpan timeSpan) {

			if (timeSpan.TotalDays >= 2)
				return string.Format(TimeStrings.TimeAgo, (int)timeSpan.TotalDays, TimeStrings.Days);

			if (timeSpan.TotalHours >= 2)
				return string.Format(TimeStrings.TimeAgo, (int)timeSpan.TotalHours, TimeStrings.Hours);

			return string.Format(TimeStrings.TimeAgo, (int)timeSpan.TotalMinutes, TimeStrings.Minutes);

		}

		public static string FormatTimeAgo(DateTime now, DateTime time) {

			var timeSpan = now - time;

			if (timeSpan.TotalDays > 60) {
				var months = ((now.Year - time.Year) * 12) + now.Month - time.Month;
				return string.Format(TimeStrings.TimeAgo, months, TimeStrings.Months);
			}

			return FormatTimeAgo(timeSpan);

		}

		public static string FormatTimeAgo(DateTime time) {
			return FormatTimeAgo(DateTime.Now, time);
		}

	}
}