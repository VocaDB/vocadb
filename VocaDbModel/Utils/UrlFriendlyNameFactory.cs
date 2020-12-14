#nullable disable

using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Utils
{
	public static class UrlFriendlyNameFactory
	{
		/// <summary>
		/// Gets an URL-friendly name from any entry name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// English or Romanized name is preferred.
		/// This method will use the original names list, which is slower and should be cached.
		/// </summary>
		public static string GetUrlFriendlyName(INameManager nameManager)
		{
			string raw = null;

			// Try English if English is the default language selection
			if (nameManager.SortNames.DefaultLanguage == ContentLanguageSelection.English)
				raw = nameManager.SortNames.English;

			// Otherwise try Romaji
			if (raw == null)
				raw = nameManager.SortNames.Romaji;

			// Try English again since there was no Romaji name
			if (raw == null)
				raw = nameManager.SortNames.English;

			// No English or Romaji names, return empty.
			if (raw == null)
				return string.Empty;

			return GetUrlFriendlyName(raw);
		}

		/// <summary>
		/// Gets an URL-friendly name from translated name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// English or Romanized name is preferred.
		/// </summary>
		public static string GetUrlFriendlyName(TranslatedString translatedString)
		{
			string raw = null;

			// Try English if English is the default language selection
			if (translatedString.DefaultLanguage == ContentLanguageSelection.English)
				raw = translatedString.English;

			// Otherwise try Romaji
			if (string.IsNullOrEmpty(raw))
				raw = translatedString.Romaji;

			// Try English again since there was no Romaji name
			if (string.IsNullOrEmpty(raw))
				raw = translatedString.English;

			// No English or Romaji names, return empty.
			if (string.IsNullOrEmpty(raw))
				return string.Empty;

			return GetUrlFriendlyName(raw);
		}

		/// <summary>
		/// Gets an URL-friendly name from any entry name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// - Spaces are replaced with dashes '-'.
		/// - All non-ASCII characters except digits, underscore '_' and dash '-' are removed.
		/// - The name is trimmed from whitespace and special characters such as underscores and dashes.
		/// - Name is truncated to 30 characters.
		/// - Name is lowercased.
		/// </summary>
		/// <param name="name">Entry name, for example "Hatsune Miku". Can be null or empty.</param>
		/// <returns>Processed name, for example "hatsune-miku". Can be empty. Cannot be null.</returns>
		public static string GetUrlFriendlyName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return string.Empty;

			// Note: perf test about 100000 iterations per second, although could be optimized/cached
			var cleanedName = Regex.Replace(name.Replace(' ', '-'), @"[^a-zA-Z0-9_-]", string.Empty);
			return cleanedName.Trim(' ', '-', '_').Truncate(30).ToLowerInvariant();
		}
	}
}
