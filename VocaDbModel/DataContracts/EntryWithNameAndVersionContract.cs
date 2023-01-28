#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts;

/// <summary>
/// Used for comments
/// </summary>
public class EntryWithNameAndVersionContract : EntryRefWithNameContract, IEntryBase
{
#nullable enable
	string IEntryBase.DefaultName => Name.DisplayName;
#nullable disable

	bool IDeletableEntry.Deleted => false;

	public EntryWithNameAndVersionContract(IEntryWithNames entry, ContentLanguagePreference languagePreference)
		: base(entry, languagePreference)
	{
		Version = entry.Version;
	}

	public int Version { get; init; }
}
