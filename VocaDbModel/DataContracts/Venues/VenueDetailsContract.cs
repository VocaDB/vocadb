using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class VenueDetailsContract : VenueContract {

		public WebLinkContract[] WebLinks { get; set; }

		public VenueDetailsContract(Venue venue, ContentLanguagePreference languagePreference) : base(venue, languagePreference) {

			WebLinks = venue.WebLinks.Select(l => new WebLinkContract(l)).ToArray();

		}

	}

}
