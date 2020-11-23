namespace VocaDb.Model.Database.Repositories
{
	/// <summary>
	/// Loads entities of specific type by ID.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IEntityLoader<out TEntity>
	{
		/// <summary>
		/// Loads an entity from the repository, checking whether the entity exists.
		/// </summary>
		/// <param name="id">Entity Id.</param>
		/// <returns>The loaded entity. Null if not found.</returns>
		TEntity Get(object id);

		/// <summary>
		/// Loads an entity from the repository, assuming the entity exists.
		/// </summary>
		/// <param name="id">Entity Id.</param>
		/// <returns>The loaded entity. Cannot be null.</returns>
		/// <remarks>
		/// This method returns a proxy that will be loaded when it's first accessed. 
		/// Accessing the proxy throws a NHibernate exception if the entity is not found.
		/// </remarks>
		TEntity Load(object id);
	}
}
