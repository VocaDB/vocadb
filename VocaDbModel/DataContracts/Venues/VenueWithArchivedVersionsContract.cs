using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{

	public class VenueWithArchivedVersionsContract : VenueContract
	{

		public ArchivedVenueVersionContract[] ArchivedVersions { get; set; }

		public VenueWithArchivedVersionsContract(Venue venue, ContentLanguagePreference languagePreference) : base(venue, languagePreference)
		{

			ArchivedVersions = venue.ArchivedVersionsManager.Versions.Select(a => new ArchivedVenueVersionContract(a)).ToArray();

		}

	}
}
