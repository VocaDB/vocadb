#nullable disable

using NHibernate;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate;

public class AlbumNHibernateRepository : NHibernateRepository<Album>, IAlbumRepository
{
	public AlbumNHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
		: base(sessionFactory, permissionContext) { }
}
