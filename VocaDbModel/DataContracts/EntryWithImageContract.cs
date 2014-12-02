using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	// Used for activity feed and comments
	// TODO: name isn't actually needed...
	public class EntryWithImageContract : EntryWithNameAndVersionContract, IEntryImageInformation {

		public EntryWithImageContract(IEntryWithNames entry, string mime, ContentLanguagePreference languagePreference) 
			: base(entry, languagePreference) {
			
			Mime = mime;

		}

		public string Mime { get; set;}

	}
}
