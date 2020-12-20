#nullable disable

using System.Collections.Generic;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Globalization
{
	public class SingleNameManager : INameManager
	{
		private readonly string _name;

		public SingleNameManager(string name)
		{
			_name = name;
		}

		public EntryNameContract GetEntryName(ContentLanguagePreference languagePreference)
		{
			return new EntryNameContract(_name);
		}

		public TranslatedString SortNames => TranslatedString.Create(_name);

		public IEnumerable<string> AllValues => new[] { _name };

		public IEnumerable<LocalizedStringWithId> NamesBase => new[] { new LocalizedStringWithId(_name, ContentLanguageSelection.Unspecified) };

		public LocalizedStringWithId FirstNameBase(ContentLanguageSelection languageSelection)
		{
			return new LocalizedStringWithId(_name, languageSelection);
		}

		public string FirstNameValue(ContentLanguageSelection languageSelection)
		{
			return _name;
		}

		public string GetAdditionalNamesStringForLanguage(ContentLanguagePreference languagePreference)
		{
			return string.Empty;
		}

		public string GetUrlFriendlyName()
		{
			return UrlFriendlyNameFactory.GetUrlFriendlyName(this);
		}

		public bool HasNameForLanguage(ContentLanguageSelection language)
		{
			return true;
		}
	}
}
