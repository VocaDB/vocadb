using System.Collections.Generic;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.Globalization {

	public interface INameManager {

		IEnumerable<string> AllValues { get; }

		IEnumerable<LocalizedStringWithId> NamesBase { get; }

		TranslatedString SortNames { get; }

		LocalizedStringWithId FirstNameBase(ContentLanguageSelection languageSelection);

		string GetAdditionalNamesStringForLanguage(ContentLanguagePreference languagePreference);

		EntryNameContract GetEntryName(ContentLanguagePreference languagePreference);

		bool HasNameForLanguage(ContentLanguageSelection language);

	}

	public interface INameManager<TName> : INameManager where TName : LocalizedStringWithId {

		IList<TName> Names { get; }

	}

}
