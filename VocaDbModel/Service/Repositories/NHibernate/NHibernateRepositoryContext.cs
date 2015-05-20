using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.Repositories.NHibernate {

	public class NHibernateRepositoryContext : IRepositoryContext {

		public IUserPermissionContext PermissionContext { get; private set; }
		public ISession Session { get; private set; }

		public NHibernateRepositoryContext(ISession session, IUserPermissionContext permissionContext) {
			Session = session;
			PermissionContext = permissionContext;
		}

		public IAuditLogger AuditLogger {
			get { return new NHibernateAuditLogger(OfType<AuditLogEntry>(), PermissionContext); }
		}

		public void Dispose() {
			Session.Dispose();
		}

		public IRepositoryContext<T2> OfType<T2>() {
			return new NHibernateRepositoryContext<T2>(Session, PermissionContext);
		}

		public IQueryable<T2> Query<T2>() {
			return OfType<T2>().Query();
		}

	}

	public class NHibernateRepositoryContext<T> : NHibernateRepositoryContext, IRepositoryContext<T> {

		public NHibernateRepositoryContext(ISession session, IUserPermissionContext permissionContext)
			: base(session, permissionContext) {

		}

		public void Delete(T entity) {
			Session.Delete(entity);
		}

		public T Get(object id) {
			return Session.Get<T>(id);
		}

		public T Load(object id) {
			return Session.Load<T>(id);
		}

		public IQueryable<T> Query() {
			return Session.Query<T>();
		}

		public void Save(T obj) {
			Session.Save(obj);
		}

		public void Update(T obj) {
			Session.Update(obj);
		}

	}

}