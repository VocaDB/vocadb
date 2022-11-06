#nullable disable

using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	[Obsolete]
	public class VenueWithArchivedVersionsContract : VenueContract
	{
		public ArchivedVenueVersionContract[] ArchivedVersions { get; init; }

		public VenueWithArchivedVersionsContract(Venue venue, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(venue, languagePreference)
		{
			ArchivedVersions = venue.ArchivedVersionsManager.Versions.Select(a => new ArchivedVenueVersionContract(a, userIconFactory)).ToArray();
		}
	}
}
