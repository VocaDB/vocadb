using System;
using System.Data;
using System.Linq;

namespace VocaDb.Model.Service.Repositories {

	public interface IRepositoryContext : IDisposable {
	
		/// <summary>
		/// Audit logger for the repository.
		/// </summary>
		IAuditLogger AuditLogger { get; }

		IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel);

		/// <summary>
		/// Returns a child context for another entity type.
		/// The unit of work (including transaction) must be shared between this parent context and the child context.
		/// </summary>
		/// <typeparam name="T2">New entity type.</typeparam>
		/// <returns>Child context for that entity type. Cannot be null.</returns>
		IRepositoryContext<T2> OfType<T2>();

		/// <summary>
		/// LINQ query against the repository.
		/// </summary>
		/// <returns>Queryable interface. Cannot be null.</returns>
		IQueryable<T2> Query<T2>();

	}

	/// <summary>
	/// Interface for an unit of work against the repository (database).
	/// The context (and the contained database session) must be disposed when no longer needed.
	/// </summary>
	/// <typeparam name="T">Type of entity.</typeparam>
	/// <remarks>
	/// Currently database session is open only during the unit of work. 
	/// This means entities returned by the query methods of this context are not valid outside the context,
	/// after the context and the session are disposed.
	/// 
	/// This might change later with Session Per Request model.
	/// </remarks>
	public interface IRepositoryContext<T> : IRepositoryContext {

		/// <summary>
		/// Deletes an entity from the repository.
		/// </summary>
		/// <param name="entity">Entity to be deleted. Cannot be null.</param>
		void Delete(T entity);

		/// <summary>
		/// Loads an entity from the repository, checking whether the entity exists.
		/// </summary>
		/// <param name="id">Entity Id.</param>
		/// <returns>The loaded entity. Null if not found.</returns>
		T Get(object id);

		/// <summary>
		/// Loads an entity from the repository, assuming the entity exists.
		/// </summary>
		/// <param name="id">Entity Id.</param>
		/// <returns>The loaded entity. Cannot be null.</returns>
		T Load(object id);
			
		/// <summary>
		/// LINQ query against the repository.
		/// </summary>
		/// <returns>Queryable interface. Cannot be null.</returns>
		IQueryable<T> Query();

		/// <summary>
		/// Persists a new entity in the repository.
		/// </summary>
		/// <param name="obj">Entity to be saved. Cannot be null.</param>
		void Save(T obj);

		/// <summary>
		/// Updates an existing entity in the repository.
		/// </summary>
		/// <param name="obj">Entity to be updated. Cannot be null.</param>
		void Update(T obj);

	}

}