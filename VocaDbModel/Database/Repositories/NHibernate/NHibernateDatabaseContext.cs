using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate
{

	public class NHibernateDatabaseContext : IDatabaseContext
	{

		public IUserPermissionContext PermissionContext { get; private set; }
		public ISession Session { get; private set; }

		public NHibernateDatabaseContext(ISession session, IUserPermissionContext permissionContext)
		{
			Session = session;
			PermissionContext = permissionContext;
		}

		public IAuditLogger AuditLogger
		{
			get { return new NHibernateAuditLogger(OfType<AuditLogEntry>(), PermissionContext); }
		}

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return new NHibernateTransaction(Session.BeginTransaction(isolationLevel));
		}

		public virtual void Dispose() => Session.Dispose();

		public void Flush() => Session.Flush();

		public IDatabaseContext<T2> OfType<T2>() where T2 : class, IDatabaseObject
		{
			return new NHibernateDatabaseContext<T2>(Session, PermissionContext);
		}

		public IQueryable<T2> Query<T2>() where T2 : class, IDatabaseObject => OfType<T2>().Query();

	}

	public class NHibernateDatabaseContext<T> : NHibernateDatabaseContext, IDatabaseContext<T>
	{

		public NHibernateDatabaseContext(ISession session, IUserPermissionContext permissionContext)
			: base(session, permissionContext)
		{

		}

		public void Delete(T entity) => Session.Delete(entity);

		public Task DeleteAsync(T entity) => Session.DeleteAsync(entity);

		public T Get(object id) => Session.Get<T>(id);

		public T Load(object id) => Session.Load<T>(id);

		public Task<T> LoadAsync(object id) => Session.LoadAsync<T>(id);

		public IQueryable<T> Query() => Session.Query<T>();

		public T Save(T obj)
		{
			Session.Save(obj);
			return obj;
		}

		public async Task<T> SaveAsync(T obj)
		{
			await Session.SaveAsync(obj);
			return obj;
		}

		public void Update(T obj) => Session.Update(obj);

		public Task UpdateAsync(T obj) => Session.UpdateAsync(obj);

	}

}