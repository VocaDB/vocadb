#nullable disable

using System;
using System.Text.RegularExpressions;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Helpers
{
	public static class DateTimeUtils
	{
		private static readonly Regex _simpleTimeRegex = new(@"(\d+)([dhm]?)");

		public static string FormatFromSeconds(int seconds)
		{
			if (seconds <= 0)
				return string.Empty;

			return DateTimeHelper.FormatMinSec(TimeSpan.FromSeconds(seconds));
		}

		public static TimeSpan ParseFromSimpleString(string timeSpanStr)
		{
			if (string.IsNullOrEmpty(timeSpanStr))
				return TimeSpan.Zero;

			var match = _simpleTimeRegex.Match(timeSpanStr);

			if (!match.Success)
				return TimeSpan.Zero;

			var quantity = int.Parse(match.Groups[1].Value);
			var unit = (match.Groups.Count >= 3 ? match.Groups[2].Value : string.Empty).ToLowerInvariant();

			return unit switch
			{
				"d" => TimeSpan.FromDays(quantity),
				"m" => TimeSpan.FromMinutes(quantity),
				_ => TimeSpan.FromHours(quantity),
			};
		}
	}
}