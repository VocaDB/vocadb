using NHibernate;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	public class EventNHibernateRepository : NHibernateRepository<ReleaseEvent>, IEventRepository
	{
		public EventNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }
	}
}
