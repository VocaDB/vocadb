using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	public class VenueNHibernateRepository : NHibernateRepository<Venue>, IVenueRepository
	{
		public VenueNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }
	}
}
