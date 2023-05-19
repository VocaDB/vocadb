using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers;

public static class LyricsHelper
{

	public static bool ContainsLyricsWithCultureCode(LyricsForSongContract[] lyrics, OptionalCultureCode cultureCode)
	{
		return lyrics.Where(l => l.CultureCodes != null)
			.Any(l => l.CultureCodes!.Any(c => c == cultureCode.CultureCode));
	}

	public static LyricsForSongContract? GetFirstLyricsForCultureCode(LyricsForSongContract[] lyrics, OptionalCultureCode cultureCode)
	{
		return lyrics.Where(l => l.CultureCodes != null)
			.First(l => l.CultureCodes!.Contains(cultureCode.CultureCode));
	}

	public static LyricsForSongContract? GetDefaultLyrics(LyricsForSongContract[] lyrics, OptionalCultureCode uiCultureCode,
		IEnumerable<OptionalCultureCode>? userLanguages,
		Lazy<IEnumerable<UserKnownLanguage>> knownLanguages)
	{
		if (!lyrics.Any())
			return null;

		var uiCulture = uiCultureCode.CultureInfo;
		if (uiCulture != null && ContainsLyricsWithCultureCode(lyrics, uiCultureCode))
		{
			return GetFirstLyricsForCultureCode(lyrics, uiCultureCode);
		}

		var userLang = userLanguages?.Select(c => c.GetCultureInfoSafe())
			.Where(c => c != null)
			.Select(l => new OptionalCultureCode(l!.TwoLetterISOLanguageName))
			.FirstOrDefault(l => ContainsLyricsWithCultureCode(lyrics, l));

		if (userLang != null && !userLang.IsEmpty)
		{
			return GetFirstLyricsForCultureCode(lyrics, userLang);
		}

		var lang = knownLanguages.Value?
			.OrderByDescending(l => l.Proficiency)
			.Select(l => l.CultureCode)
			.FirstOrDefault(l => ContainsLyricsWithCultureCode(lyrics, l));

		if (lang != null)
		{
			return GetFirstLyricsForCultureCode(lyrics, lang);
		}

		var original = lyrics.FirstOrDefault(l => l.TranslationType == TranslationType.Original);

		return original ?? lyrics.First();
	}
}
