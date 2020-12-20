#nullable disable

using System;
using System.Linq;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code
{
	public class SloganGenerator
	{
		private static readonly string[] defaultSlogans = {
			"Telling you who's whoo.",
			"1st place to check.",
			"We got APIs and ApiMikus.",
			"Use the search, Luka!",
			"shu-t up and enjoy",
			"Now with 39% more Miku",
			"Our site uses Micookies",
			"Passwords secured with HMIC"
		};

		private static string[] s_slogans;

		private static string[] GetValues()
		{
			if (s_slogans == null)
			{
				var config = AppConfig.GetSlogansSection();
				s_slogans = config?.Slogans?.Select(s => s.Value).ToArray() ?? defaultSlogans;
			}

			return s_slogans;
		}

		public static string Generate()
		{
			var values = GetValues();

			if (!values.Any())
				return string.Empty;

			var result = values[new Random().Next(values.Length)];

			return result;
		}
	}
}