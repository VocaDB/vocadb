using NHibernate;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	/// <summary>
	/// NHibernate database context (session) which supports the session per request pattern, meaning the database session is persisted during the HTTP request.
	/// </summary>
	/// <typeparam name="T">Entity type.</typeparam>
	public class NHibernateSessionPerRequestContext<T> : NHibernateDatabaseContext<T>
	{
		public NHibernateSessionPerRequestContext(ISession session, IUserPermissionContext permissionContext)
			: base(session, permissionContext) { }

		// NO-OP
		public override void Dispose() { }
	}
}
