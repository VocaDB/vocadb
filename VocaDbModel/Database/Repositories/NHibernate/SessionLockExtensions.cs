using NHibernate;

namespace VocaDb.Model.Database.Repositories.NHibernate;

public static class SessionLockExtensions
{
	/// <summary>
	/// See <see cref="IDatabaseContext{T}.LoadForUpdate"/>.
	/// </summary>
	public static T LoadForUpdate<T>(this ISession session, object id)
	{
		// Get only locks an entity that is already loaded, without reloading it
		if (GetLoadedEntity<T>(session, id) is T loaded)
		{
			session.Refresh(loaded, LockMode.Upgrade);
			return loaded;
		}

		return session.Get<T>(id, LockMode.Upgrade) ?? throw new ObjectNotFoundException(id, typeof(T));
	}

	public static async Task<T> LoadForUpdateAsync<T>(this ISession session, object id)
	{
		if (GetLoadedEntity<T>(session, id) is T loaded)
		{
			await session.RefreshAsync(loaded, LockMode.Upgrade);
			return loaded;
		}

		return await session.GetAsync<T>(id, LockMode.Upgrade) ?? throw new ObjectNotFoundException(id, typeof(T));
	}

	/// <summary>
	/// Loads two entities for update, lower id first to avoid deadlocks.
	/// </summary>
	public static (T First, T Second) LoadForUpdate<T>(this ISession session, int firstId, int secondId)
	{
		if (firstId <= secondId)
		{
			var first = session.LoadForUpdate<T>(firstId);
			return (first, session.LoadForUpdate<T>(secondId));
		}

		var second = session.LoadForUpdate<T>(secondId);
		return (session.LoadForUpdate<T>(firstId), second);
	}

	private static object? GetLoadedEntity<T>(ISession session, object id)
	{
		var impl = session.GetSessionImplementation();
		var persister = impl.Factory.GetEntityPersister(typeof(T).FullName);
		return impl.PersistenceContext.GetEntity(impl.GenerateEntityKey(id, persister));
	}
}
