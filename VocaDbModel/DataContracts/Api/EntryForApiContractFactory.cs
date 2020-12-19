#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Api
{
	/// <summary>
	/// Creates instances of <see cref="EntryForApiContract"/>.
	/// </summary>
	public class EntryForApiContractFactory
	{
		private readonly IAggregatedEntryImageUrlFactory _thumbPersister;

		public EntryForApiContractFactory(IAggregatedEntryImageUrlFactory thumbPersister)
		{
			this._thumbPersister = thumbPersister;
		}

		public EntryForApiContract Create(IEntryWithNames entry, EntryOptionalFields includedFields, ContentLanguagePreference languagePreference)
		{
			return EntryForApiContract.Create(entry, languagePreference, _thumbPersister, includedFields);
		}
	}
}
