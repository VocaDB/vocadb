#nullable disable

using NHibernate;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	public class DiscussionFolderNHibernateRepository : NHibernateRepository<DiscussionFolder>, IDiscussionFolderRepository
	{
		public DiscussionFolderNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }
	}
}
