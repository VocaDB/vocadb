using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Api {

	/// <summary>
	/// Creates instances of <see cref="EntryForApiContract"/>.
	/// </summary>
	public class EntryForApiContractFactory {

		private readonly IEntryImagePersisterOld imagePersisterOld;
		private readonly IEntryThumbPersister thumbPersister;

		public EntryForApiContractFactory(IEntryImagePersisterOld imagePersisterOld, IEntryThumbPersister thumbPersister) {
			this.imagePersisterOld = imagePersisterOld;
			this.thumbPersister = thumbPersister;
		}

		public EntryForApiContract Create(IEntryWithNames entry, EntryOptionalFields includedFields, ContentLanguagePreference languagePreference, bool ssl) {
			
			return EntryForApiContract.Create(entry, languagePreference, thumbPersister, imagePersisterOld, ssl, includedFields);

		}

	}
}
