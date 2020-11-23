using System.Collections.Generic;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Globalization
{
	public interface INameManager
	{
		/// <summary>
		/// List of all name values.
		/// This list includes translated sort name as well as aliases, 
		/// meaning union of <see cref="SortNames"/> and <see cref="NamesBase"/>.
		/// Duplicates are excluded.
		/// Cannot be null.
		/// This list is not persisted to the database.
		/// </summary>
		IEnumerable<string> AllValues { get; }

		/// <summary>
		/// List of names.
		/// This is the list of assigned names for a entry.
		/// 
		/// This list does not automatically include the sort names:
		/// the entry might have a translated sort name (<see cref="SortNames"/>)
		/// even though this list is empty.
		/// 
		/// The sort names are generated from this list.
		/// This list is persisted to the database.
		/// 
		/// Cannot be null.
		/// </summary>
		IEnumerable<LocalizedStringWithId> NamesBase { get; }

		TranslatedString SortNames { get; }

		/// <summary>
		/// Gets the value of the first name matching a language selection.
		/// Language substitutions are *not* applied.
		/// </summary>
		/// <param name="languageSelection">Language selection.</param>
		/// <returns>Name value. Can be null if there is no name for the specfied language selection.</returns>
		string FirstNameValue(ContentLanguageSelection languageSelection);

		/// <summary>
		/// Gets a comma-separated list of additional names for a specific language.
		/// 
		/// For example, if the entry has names A, B and C for Japanese, Romaji and English respectively,
		/// the return value for English would be "A, B", and for Japanese it would be "B, C".
		/// </summary>
		/// <param name="languagePreference">Language preference indicating the primary display name (which is EXCLUDED from this string).</param>
		/// <returns>Additional names string. Cannot be null.</returns>
		string GetAdditionalNamesStringForLanguage(ContentLanguagePreference languagePreference);

		/// <summary>
		/// Gets entry name (with both primary display name and list of additional names)
		/// for a specific language.
		/// </summary>
		/// <param name="languagePreference">Language preference.</param>
		/// <returns>Entry name. Cannot be null.</returns>
		EntryNameContract GetEntryName(ContentLanguagePreference languagePreference);

		/// <summary>
		/// Gets the URL-friendly name for the entry.
		/// </summary>
		/// <returns>URL-friendly name. Can be empty if there is none. Cannot be null.</returns>
		string GetUrlFriendlyName();

		/// <summary>
		/// Whether the entry has a name for a specific language selection.
		/// Language substitutions are *not* applied.
		/// </summary>
		/// <param name="language">Language selection.</param>
		/// <returns>True if a name exists for the language selection.</returns>
		bool HasNameForLanguage(ContentLanguageSelection language);
	}

	public interface INameManager<TName> : INameManager where TName : LocalizedStringWithId
	{
		IList<TName> Names { get; }
	}
}
