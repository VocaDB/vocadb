using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class LocalizedStringHelper {

		/// <summary>
		/// Gets the best matched string based on language preference and primary language selection.
		/// </summary>
		/// <typeparam name="T">Type of localized string.</typeparam>
		/// <param name="strings">List of strings. Can be null or empty, in which case null is returned.</param>
		/// <param name="languagePreference">User's language preference.</param>
		/// <param name="primaryLanguageSelection">Entry's primary language selection.</param>
		/// <returns>Best matched string. Can be null, but only if the source list is null or empty.</returns>
		/// <remarks>
		/// Selection is done as follows:
		/// 1) The first string matching user's language preference, English is substituted for Romaji and vice versa.
		/// 2) The first string matching the primary language selection, unless it's "Unspecified".
		/// 3) The first string in any language.
		/// </remarks>
		public static T GetBestMatch<T>(IList<T> strings, ContentLanguagePreference languagePreference, ContentLanguageSelection primaryLanguageSelection)
			where T: class, ILocalizedString {
			
			if (strings == null || !strings.Any())
				return null;

			var languagePreferenceAsSelection = languagePreference == ContentLanguagePreference.Default ? primaryLanguageSelection : (ContentLanguageSelection)languagePreference;
			var match = strings.FirstOrDefault(s => s.Language == languagePreferenceAsSelection);

			if (match != null)
				return match;

			// Substitute English for Romaji and vice versa
			if (languagePreference == ContentLanguagePreference.English) {
				match = strings.FirstOrDefault(s => s.Language == ContentLanguageSelection.Romaji);				
			}

			if (match != null)
				return match;

			if (languagePreference == ContentLanguagePreference.Romaji) {
				match = strings.FirstOrDefault(s => s.Language == ContentLanguageSelection.English);				
			}

			if (match != null)
				return match;

			if (primaryLanguageSelection != ContentLanguageSelection.Unspecified) {
				match = strings.FirstOrDefault(s => s.Language == primaryLanguageSelection);				
			}

			return match ?? strings.First();

		}

		public static IEnumerable<T> Order<T>(IEnumerable<T> entries, ContentLanguagePreference languagePreference) where T : IEntryWithNames {

			if (languagePreference == ContentLanguagePreference.Japanese)
				return entries.OrderBy(e => e.Names.SortNames.Japanese);
			else if (languagePreference == ContentLanguagePreference.English)
				return entries.OrderBy(e => e.Names.SortNames.English);
			else
				return entries.OrderBy(e => e.Names.SortNames.Japanese);		

		}

		public static IEnumerable<LocalizedStringContract> SkipNullAndEmpty(string original, string romaji, string english) {

			var names = new List<LocalizedStringContract>();

			if (!string.IsNullOrWhiteSpace(original))
				names.Add(new LocalizedStringContract(original.Trim(), ContentLanguageSelection.Japanese));

			if (!string.IsNullOrWhiteSpace(romaji))
				names.Add(new LocalizedStringContract(romaji.Trim(), ContentLanguageSelection.Romaji));

			if (!string.IsNullOrWhiteSpace(english))
				names.Add(new LocalizedStringContract(english.Trim(), ContentLanguageSelection.English));

			return names;

		}

	}
}
