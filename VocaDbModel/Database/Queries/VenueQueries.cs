using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries {

	public class VenueQueries : QueriesBase<IVenueRepository, Venue> {

		public VenueQueries(IVenueRepository venueRepository, IUserPermissionContext permissionContext)
			: base(venueRepository, permissionContext) { }

		public VenueDetailsContract GetDetails(int id) {

			return new VenueDetailsContract(new Venue()/* TODO */, LanguagePreference);

		}

		public VenueForEditContract GetForEdit(int id) {

			return new VenueForEditContract(new Venue()/* TODO */, LanguagePreference);

		}

		public VenueWithArchivedVersionsContract GetWithArchivedVersions(int id) {

			return new VenueWithArchivedVersionsContract(new Venue()/* TODO */, LanguagePreference);

		}

	}

}
