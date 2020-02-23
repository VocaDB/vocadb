using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class VenueWithEventsContract : VenueContract {
		
		public ReleaseEventContract[] Events { get; set; }

		public VenueWithEventsContract() { }

		public VenueWithEventsContract(Venue venue, ContentLanguagePreference languagePreference)
			: base(venue, languagePreference) {

			Events = venue.Events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();

		}

		public VenueWithEventsContract(Venue series, IEnumerable<ReleaseEvent> events, ContentLanguagePreference languagePreference)
			: base(series, languagePreference) {

			ParamIs.NotNull(() => events);

			Events = events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();

		}

	}

}
