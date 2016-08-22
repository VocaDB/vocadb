using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Globalization;

namespace VocaDb.Model.Domain.Globalization {

	public static class InterfaceLanguage {

		private static readonly ImmutableHashSet<string> twoLetterLanguageCodes;

		static InterfaceLanguage() {
			twoLetterLanguageCodes = ImmutableHashSet.CreateRange(Cultures.Select(c => c.TwoLetterISOLanguageName));
		}

		public static IEnumerable<CultureInfo> Cultures => LanguageCodes.Select(CultureInfo.GetCultureInfo);

		/// <summary>
		/// Gets the ISO 639-1 two letter language code matching a culture,
		/// if that culture is supported as display language on the site.
		/// </summary>
		/// <param name="culture">Culture to be tested. Cannot be null.</param>
		/// <returns>Language code matching the culture. If there was no match, this returns an empty string.</returns>
		public static string GetAvailableLanguageCode(CultureInfo culture) {
			return IsValidCulture(culture) ? culture.TwoLetterISOLanguageName : string.Empty;
		}

		private static bool IsValidCulture(CultureInfo culture) {
			return twoLetterLanguageCodes.Contains(culture.TwoLetterISOLanguageName);
		}

		public static readonly string[] LanguageCodes = {
			"en-US", "de-DE", "es", "fi-Fi", "pt", "ru-RU", "ja-JP", "zh-Hans"
		};

		public static readonly string[] UserLanguageCodes = {
			"de-DE", "en-GB", "en-US", "es", "fi-Fi", "fil-PH", "fr-FR", "id-ID", "it-IT", "ko-KR", "nl-NL", "nn-NO", "pl-PL", "pt", "ru-RU", "sv-SE", "ja-JP", "zh-Hans"
		};

		public static IEnumerable<CultureInfo> UserLanguageCultures => UserLanguageCodes.Select(CultureInfo.GetCultureInfo);

	}
}
