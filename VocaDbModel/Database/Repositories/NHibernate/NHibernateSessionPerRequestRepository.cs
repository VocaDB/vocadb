using NHibernate;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate
{

	/// <summary>
	/// NHibernate repository which supports the session per request pattern, meaning the database session is persisted during the HTTP request.
	/// </summary>
	/// <typeparam name="T">Entity type.</typeparam>
	public class NHibernateSessionPerRequestRepository<T> : NHibernateRepository<T>
		where T : class, IDatabaseObject
	{

		private readonly ISession session;

		protected override NHibernateDatabaseContext<T> OpenSessionForContext()
		{
			return new NHibernateSessionPerRequestContext<T>(session, PermissionContext);
		}

		public NHibernateSessionPerRequestRepository(ISession session, ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext)
		{

			this.session = session;

		}

	}
}
