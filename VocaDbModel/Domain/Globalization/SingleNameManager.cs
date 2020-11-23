using System.Collections.Generic;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Globalization
{

	public class SingleNameManager : INameManager
	{

		private readonly string name;

		public SingleNameManager(string name)
		{
			this.name = name;
		}

		public EntryNameContract GetEntryName(ContentLanguagePreference languagePreference)
		{
			return new EntryNameContract(name);
		}

		public TranslatedString SortNames
		{
			get
			{
				return TranslatedString.Create(name);
			}
		}

		public IEnumerable<string> AllValues
		{
			get
			{
				return new[] { name };
			}
		}

		public IEnumerable<LocalizedStringWithId> NamesBase
		{
			get
			{
				return new[] { new LocalizedStringWithId(name, ContentLanguageSelection.Unspecified) };
			}
		}

		public LocalizedStringWithId FirstNameBase(ContentLanguageSelection languageSelection)
		{
			return new LocalizedStringWithId(name, languageSelection);
		}

		public string FirstNameValue(ContentLanguageSelection languageSelection)
		{
			return name;
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
