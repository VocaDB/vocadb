using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	public class TagNHibernateRepository : NHibernateRepository<Tag>, ITagRepository
	{
		public TagNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }
	}
}
