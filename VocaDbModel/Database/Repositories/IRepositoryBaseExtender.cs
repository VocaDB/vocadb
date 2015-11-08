using System;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories {

	public static class IRepositoryBaseExtender {

		public static void UpdateEntity<TEntity, TRepositoryContext>(this IRepositoryBase<TRepositoryContext> repository, int id, 
			Action<TRepositoryContext, TEntity> func, PermissionToken permissionFlags, IUserPermissionContext permissionContext, 
			bool skipLog = false) where TRepositoryContext : IDatabaseContext {

			var typeName = typeof(TEntity).Name;

			permissionContext.VerifyPermission(permissionFlags);

			repository.HandleTransaction(session => {

				session.AuditLogger.SysLog(string.Format("is about to update {0} with Id {1}", typeName, id));

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					session.AuditLogger.AuditLog("updating " + entity);
				else
					session.AuditLogger.SysLog("updating " + entity);

				func(session, entity);

				session.OfType<TEntity>().Update(entity);

			}, "Unable to update " + typeName);

		}

	}

}
