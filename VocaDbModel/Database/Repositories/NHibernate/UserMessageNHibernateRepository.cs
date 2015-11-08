using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Database.Repositories.NHibernate {

	public class UserMessageNHibernateRepository : NHibernateRepository<UserMessage>, IUserMessageRepository {

		public UserMessageNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }

	}

}
