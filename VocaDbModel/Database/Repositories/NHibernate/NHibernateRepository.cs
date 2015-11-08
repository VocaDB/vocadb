using System;
using NHibernate;
using NLog;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories.NHibernate {

	public class NHibernateRepository : IRepository {
	
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		public IUserPermissionContext PermissionContext { get; private set; }
		public ISessionFactory SessionFactory { get; private set; }
	
		protected NHibernateDatabaseContext OpenSessionForContext() {
			return new NHibernateDatabaseContext(SessionFactory.OpenSession(), PermissionContext);
		}

		public NHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) {
			SessionFactory = sessionFactory;
			PermissionContext = permissionContext;
		}

		public TResult HandleQuery<TResult>(Func<IDatabaseContext, TResult> func, string failMsg = "Unexpected NHibernate error") {
			try {
				using (var ctx = OpenSessionForContext()) {
					return func(ctx);
				}
			} catch (ObjectNotFoundException x) {
				log.Error(x.Message);
				throw;
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}
		}

		public TResult HandleTransaction<TResult>(Func<IDatabaseContext, TResult> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var ctx = OpenSessionForContext())
				using (var tx = ctx.Session.BeginTransaction()) {

					var val = func(ctx);
					tx.Commit();
					return val;

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		public void HandleTransaction(Action<IDatabaseContext> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var ctx = OpenSessionForContext())
				using (var tx = ctx.Session.BeginTransaction()) {

					func(ctx);
					tx.Commit();

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

	}

	public abstract class NHibernateRepository<T> : IRepository<T> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		public IUserPermissionContext PermissionContext { get; private set; }
		public ISessionFactory SessionFactory { get; private set; }

		protected NHibernateDatabaseContext<T> OpenSessionForContext() {
			return new NHibernateDatabaseContext<T>(SessionFactory.OpenSession(), PermissionContext);
		}

		protected NHibernateRepository(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) {
			SessionFactory = sessionFactory;
			PermissionContext = permissionContext;
		}

		public TResult HandleQuery<TResult>(Func<IDatabaseContext<T>, TResult> func, string failMsg = "Unexpected NHibernate error") {
			try {
				using (var ctx = OpenSessionForContext()) {
					return func(ctx);
				}
			} catch (ObjectNotFoundException x) {
				log.Error(x.Message);
				throw;
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}
		}

		public TResult HandleTransaction<TResult>(Func<IDatabaseContext<T>, TResult> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var ctx = OpenSessionForContext())
				using (var tx = ctx.Session.BeginTransaction()) {

					var val = func(ctx);
					tx.Commit();
					return val;

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		public void HandleTransaction(Action<IDatabaseContext<T>> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var ctx = OpenSessionForContext())
				using (var tx = ctx.Session.BeginTransaction()) {

					func(ctx);
					tx.Commit();

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

	}

}
