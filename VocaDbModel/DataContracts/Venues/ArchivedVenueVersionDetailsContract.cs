using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues {

	public class ArchivedVenueVersionDetailsContract {

		public ArchivedVenueVersionContract ArchivedVersion { get; set; }

		public ArchivedObjectVersionContract ComparedVersion { get; set; }

		public int ComparedVersionId { get; set; }

		public string Name { get; set; }

		public VenueContract Venue { get; set; }

		public ComparedVenueContract Versions { get; set; }

		public ArchivedVenueVersionDetailsContract(ArchivedVenueVersion archived, ArchivedVenueVersion comparedVersion, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedVenueVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedVenueVersionContract(comparedVersion) : null;
			Venue = new VenueContract(archived.Entry, languagePreference);
			Name = Venue.Name;

			Versions = ComparedVenueContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;

		}

	}

}
