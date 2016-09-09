using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers {

	public static class LyricsHelper {

		private static CultureInfo GetCultureOrNull(string code) {
			try {
				return CultureInfo.GetCultureInfo(code);
			} catch (CultureNotFoundException) {
				return null;
			}
		}

		public static LyricsForSongContract GetDefaultLyrics(LyricsForSongContract[] lyrics, string uiCultureCode, 
			IEnumerable<string> userLanguages, 
			Lazy<IEnumerable<UserKnownLanguage>> knownLanguages) {

			if (!lyrics.Any())
				return null;

			var dict = lyrics.Where(l => !string.IsNullOrEmpty(l.CultureCode)).Distinct(l => l.CultureCode).ToDictionary(l => l.CultureCode);

			var uiCulture = !string.IsNullOrEmpty(uiCultureCode) ? CultureInfo.GetCultureInfo(uiCultureCode) : null;
			if (uiCulture != null && dict.ContainsKey(uiCulture.TwoLetterISOLanguageName)) {
				return dict[uiCulture.TwoLetterISOLanguageName];
			}

			var userLang = userLanguages?.Select(GetCultureOrNull)
				.Where(c => c != null)
				.Select(l => l.TwoLetterISOLanguageName)
				.FirstOrDefault(l => dict.ContainsKey(l));

			if (!string.IsNullOrEmpty(userLang)) {
				return dict[userLang];
			}

			var lang = knownLanguages.Value?.OrderByDescending(l => l.Proficiency).FirstOrDefault(l => dict.ContainsKey(l.CultureCode));

			if (lang != null) {
				return dict[lang.CultureCode];
			}

			var original = lyrics.FirstOrDefault(l => l.TranslationType == TranslationType.Original);

			return original ?? lyrics.First();

		}

	}
}
