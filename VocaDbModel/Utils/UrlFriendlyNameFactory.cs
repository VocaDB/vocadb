using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Utils {

	public static class UrlFriendlyNameFactory {

		/// <summary>
		/// Gets an URL-friendly name from any entry name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// English or Romanized name is preferred.
		/// </summary>
		public static string GetUrlFriendlyName(ITranslatedString names) {

			string raw;

			// Use Romaji if it's specified and the original language is either Japanese or Romaji.
			if ((names.DefaultLanguage == ContentLanguageSelection.Romaji || names.DefaultLanguage == ContentLanguageSelection.Japanese) 
				&& !string.IsNullOrEmpty(names.Romaji))
				raw = names.Romaji;
			else
				raw = names.English;

			return GetUrlFriendlyName(raw);

		}

		/// <summary>
		/// Gets an URL-friendly name from any entry name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// - Spaces are replaced with dashes '-'.
		/// - All non-ASCII characters except digits, underscore '_' and dash '-' are removed.
		/// - Name is truncated to 30 characters.
		/// - Name is lowercased.
		/// </summary>
		/// <param name="name">Entry name, for example "Hatsune Miku". Can be null or empty.</param>
		/// <returns>Processed name, for example "hatsune-miku". Can be empty. Cannot be null.</returns>
		public static string GetUrlFriendlyName(string name) {

			if (string.IsNullOrEmpty(name))
				return string.Empty;

			var cleanedName = Regex.Replace(name.Replace(' ', '-'), @"[^a-zA-Z0-9_-]", string.Empty);
			return cleanedName.Truncate(30).ToLowerInvariant();

		}

	}

}
