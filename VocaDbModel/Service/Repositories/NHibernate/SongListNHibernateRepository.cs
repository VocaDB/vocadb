using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.Repositories.NHibernate {

	public class SongListNHibernateRepository : NHibernateRepository<SongList>, ISongListRepository {

		public SongListNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

	}

}
