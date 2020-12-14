#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagEntryMappingContract
	{
		public TagEntryMappingContract() { }

		public TagEntryMappingContract(EntryTypeToTagMapping mapping, ContentLanguagePreference languagePreference)
		{
			EntryType = mapping.EntryTypeAndSubType;
			Tag = new TagBaseContract(mapping.Tag, languagePreference);
		}

		public EntryTypeAndSubType EntryType { get; set; }

		public TagBaseContract Tag { get; set; }
	}
}
