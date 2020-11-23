using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Database.Repositories.NHibernate
{

	public class TagNHibernateSessionPerRequestRepository : NHibernateSessionPerRequestRepository<Tag>, ITagRepository
	{

		public TagNHibernateSessionPerRequestRepository(ISession session, ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(session, sessionFactory, permissionContext) { }

	}
}
