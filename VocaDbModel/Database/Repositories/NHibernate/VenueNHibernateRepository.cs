using NHibernate;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate {

	public class VenueNHibernateRepository : NHibernateRepository<Venue>, IVenueRepository {

		public VenueNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }

	}

}
