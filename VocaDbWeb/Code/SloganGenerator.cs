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
			"Odds&Entries",
			"Bells and candy canes for DECO*27ration",
			"AiDee and password required for login",
			"Our site uses Micookies",
			"Do yuno Yunosuke?",
			"MEIKO an account today",
			"Got some assignments from UNI",
			"CYBER SONGMAN, CYBER ALBUMMAN",
			"CATS RULE THE WORLD",
			"That Flower is called Lily",
			"Passwords secured with HMIC"
		};

		private static string[]? s_slogans;

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
