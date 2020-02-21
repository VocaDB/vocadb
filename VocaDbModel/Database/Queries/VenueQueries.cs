using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries {

	public class VenueQueries : QueriesBase<IVenueRepository, Venue> {

		public VenueQueries(IVenueRepository venueRepository, IUserPermissionContext permissionContext)
			: base(venueRepository, permissionContext) { }

	}

}
