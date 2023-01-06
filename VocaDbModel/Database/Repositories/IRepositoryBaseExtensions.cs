#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Repositories;

public static class IRepositoryBaseExtensions
{
	public static void UpdateEntity<TEntity, TRepositoryContext>(this IRepositoryBase<TRepositoryContext> repository, int id,
		Action<TRepositoryContext, TEntity> func, PermissionToken permissionFlags, IUserPermissionContext permissionContext,
		bool skipLog = false)
		where TEntity : class, IDatabaseObject
		where TRepositoryContext : IDatabaseContext
	{
		var typeName = typeof(TEntity).Name;

		permissionContext.VerifyPermission(permissionFlags);

		repository.HandleTransaction(session =>
		{
			session.AuditLogger.SysLog($"is about to update {typeName} with Id {id}");

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
