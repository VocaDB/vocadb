using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Venue {

	public class Versions : Versions<VenueContract> {

		public Versions() { }

		public Versions(VenueWithArchivedVersionsContract contract, IEnumTranslations translator) : base(contract, contract.ArchivedVersions, translator) { }

	}

}