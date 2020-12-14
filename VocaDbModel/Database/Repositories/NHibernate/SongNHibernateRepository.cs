#nullable disable

using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	public class SongNHibernateRepository : NHibernateRepository<Song>, ISongRepository
	{
		public SongNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) { }
	}
}
