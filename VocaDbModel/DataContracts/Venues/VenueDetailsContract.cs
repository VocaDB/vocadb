using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class VenueDetailsContract : VenueContract {

		public VenueDetailsContract(Venue venue, ContentLanguagePreference languagePreference) : base(venue, languagePreference) { }

	}

}
