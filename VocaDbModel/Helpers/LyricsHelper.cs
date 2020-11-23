using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers
{

	public static class LyricsHelper
	{

		public static LyricsForSongContract GetDefaultLyrics(LyricsForSongContract[] lyrics, OptionalCultureCode uiCultureCode,
			IEnumerable<OptionalCultureCode> userLanguages,
			Lazy<IEnumerable<UserKnownLanguage>> knownLanguages)
		{

			if (!lyrics.Any())
				return null;

			var dict = lyrics.Where(l => !string.IsNullOrEmpty(l.CultureCode)).Distinct(l => l.CultureCode).ToDictionary(l => l.CultureCode);

			var uiCulture = uiCultureCode.CultureInfo;
			if (uiCulture != null && dict.ContainsKey(uiCulture.TwoLetterISOLanguageName))
			{
				return dict[uiCulture.TwoLetterISOLanguageName];
			}

			var userLang = userLanguages?.Select(c => c.GetCultureInfoSafe())
				.Where(c => c != null)
				.Select(l => l.TwoLetterISOLanguageName)
				.FirstOrDefault(l => dict.ContainsKey(l));

			if (!string.IsNullOrEmpty(userLang))
			{
				return dict[userLang];
			}

			var lang = knownLanguages.Value?
				.OrderByDescending(l => l.Proficiency)
				.Select(l => l.CultureCode.CultureCode)
				.FirstOrDefault(l => dict.ContainsKey(l));

			if (lang != null)
			{
				return dict[lang];
			}

			var original = lyrics.FirstOrDefault(l => l.TranslationType == TranslationType.Original);

			return original ?? lyrics.First();

		}

	}
}
