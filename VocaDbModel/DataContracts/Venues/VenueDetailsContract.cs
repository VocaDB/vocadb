using System.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class VenueDetailsContract : VenueContract {
		
		public ReleaseEventContract[] Events { get; set; }

		public VenueDetailsContract(Venue venue, ContentLanguagePreference languagePreference) : base(venue, languagePreference, true) {
			
			Events = venue.Events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();

		}

	}

}
