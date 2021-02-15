#nullable disable

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Globalization;

namespace VocaDb.Model.Domain.Globalization
{
	public static class InterfaceLanguage
	{
		private static readonly ImmutableHashSet<string> s_twoLetterLanguageCodes;

		static InterfaceLanguage()
		{
			s_twoLetterLanguageCodes = ImmutableHashSet.CreateRange(Cultures.Cultures.Select(c => c.TwoLetterISOLanguageName));
		}

		public static CultureCollection Cultures => new CultureCollection(LanguageCodes);
		public const string DefaultCultureCode = "en-US";
		public const string DefaultLanguageCode = "en";

		/// <summary>
		/// Gets the ISO 639-1 two letter language code matching a culture,
		/// if that culture is supported as display language on the site.
		/// </summary>
		/// <param name="culture">Culture to be tested. Cannot be null.</param>
		/// <returns>Language code matching the culture. If there was no match, this returns an empty string.</returns>
		public static string GetAvailableLanguageCode(CultureInfo culture)
		{
			return IsValidUserInterfaceCulture(culture) ? culture.TwoLetterISOLanguageName : string.Empty;
		}

		public static bool IsValidUserInterfaceCulture(CultureInfo culture)
		{
			return s_twoLetterLanguageCodes.Contains(culture.TwoLetterISOLanguageName);
		}

		public static readonly string[] LanguageCodes = {
			"en-US", "de-DE", "es", "fi-Fi", "pt", "ru-RU", "ja-JP", "zh-Hans"
		};

		private static readonly string[] UserLanguageCodes = {
			"de", "en", "es", "fi", "fil", "fr", "id", "it", "ko", "nl", "no", "pl", "pt", "ru", "sv", "th", "ja", "zh"
		};

		public static CultureCollection UserLanguageCultures => new CultureCollection(UserLanguageCodes);

		public static IEnumerable<string> TwoLetterLanguageCodes => s_twoLetterLanguageCodes;
	}
}
