using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate {

	public class NHibernateDatabaseContext : IDatabaseContext {

		public IUserPermissionContext PermissionContext { get; private set; }
		public ISession Session { get; private set; }

		public NHibernateDatabaseContext(ISession session, IUserPermissionContext permissionContext) {
			Session = session;
			PermissionContext = permissionContext;
		}

		public IAuditLogger AuditLogger {
			get { return new NHibernateAuditLogger(OfType<AuditLogEntry>(), PermissionContext); }
		}

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel) {
			return new NHibernateTransaction(Session.BeginTransaction(isolationLevel));
		}

		public virtual void Dispose() {
			Session.Dispose();
		}

		public void Flush() {
			Session.Flush();
		}

		public IDatabaseContext<T2> OfType<T2>() {
			return new NHibernateDatabaseContext<T2>(Session, PermissionContext);
		}

		public IQueryable<T2> Query<T2>() {
			return OfType<T2>().Query();
		}

	}

	public class NHibernateDatabaseContext<T> : NHibernateDatabaseContext, IDatabaseContext<T> {

		public NHibernateDatabaseContext(ISession session, IUserPermissionContext permissionContext)
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