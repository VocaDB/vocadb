using System;
using System.Text.RegularExpressions;

namespace VocaDb.Web.Helpers {

	public static class DateTimeUtils {

		private static readonly Regex simpleTimeRegex = new Regex(@"(\d+)([dhm]?)");

		public static string FormatFromSeconds(int seconds) {

			if (seconds <= 0)
				return string.Empty;

			return TimeSpan.FromSeconds(seconds).ToString(@"m\:ss");

		}

		public static TimeSpan ParseFromSimpleString(string timeSpanStr) {

			if (string.IsNullOrEmpty(timeSpanStr))
				return TimeSpan.Zero;

			var match = simpleTimeRegex.Match(timeSpanStr);

			if (!match.Success)
				return TimeSpan.Zero;

			var quantity = int.Parse(match.Groups[1].Value);
			var unit = (match.Groups.Count >= 3 ? match.Groups[2].Value : string.Empty).ToLowerInvariant();

			switch (unit) {
				case "d":
					return TimeSpan.FromDays(quantity);
				case "m":
					return TimeSpan.FromMinutes(quantity);
				default:
					return TimeSpan.FromHours(quantity);
			}

		}

	}

}