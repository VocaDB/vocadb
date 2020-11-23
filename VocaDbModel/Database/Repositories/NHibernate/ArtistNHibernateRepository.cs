using NHibernate;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate
{

	public class ArtistNHibernateRepository : NHibernateRepository<Artist>, IArtistRepository
	{

		public ArtistNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }

	}

}
