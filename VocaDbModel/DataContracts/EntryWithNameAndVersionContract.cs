#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts
{
	/// <summary>
	/// Used for comments
	/// </summary>
	public class EntryWithNameAndVersionContract : EntryRefWithNameContract, IEntryBase
	{
		string IEntryBase.DefaultName
		{
			get { return Name.DisplayName; }
		}

		bool IDeletableEntry.Deleted
		{
			get { return false; }
		}

		public EntryWithNameAndVersionContract(IEntryWithNames entry, ContentLanguagePreference languagePreference)
			: base(entry, languagePreference)
		{
			Version = entry.Version;
		}

		public int Version { get; set; }
	}
}
