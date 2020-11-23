using System;
using System.Threading.Tasks;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Database.Repositories
{
	public interface IRepositoryBase
	{
	}

	public interface IRepositoryBase<TRepositoryContext> : IRepositoryBase
	{
		/// <summary>
		/// Runs an unit of work that queries the database without saving anything. No explicit transaction will be opened.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		TResult HandleQuery<TResult>(Func<TRepositoryContext, TResult> func, string failMsg = "Unexpected database error");

		/// <summary>
		/// Runs an unit of work that queries the database without saving anything. No explicit transaction will be opened.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		Task<TResult> HandleQueryAsync<TResult>(Func<TRepositoryContext, Task<TResult>> func, string failMsg = "Unexpected database error");

		/// <summary>
		/// Runs an unit of work that does not return anything, inside an explicit transaction.
		/// </summary>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		void HandleTransaction(Action<TRepositoryContext> func, string failMsg = "Unexpected database error");

		Task HandleTransactionAsync(Func<TRepositoryContext, Task> func, string failMsg = "Unexpected database error");

		/// <summary>
		/// Runs an unit of work that queries the database, inside an explicit transaction.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		TResult HandleTransaction<TResult>(Func<TRepositoryContext, TResult> func, string failMsg = "Unexpected database error");

		/// <summary>
		/// Runs an unit of work that queries the database, inside an explicit transaction.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		Task<TResult> HandleTransactionAsync<TResult>(Func<TRepositoryContext, Task<TResult>> func, string failMsg = "Unexpected database error");
	}

	public interface IRepository : IRepositoryBase<IDatabaseContext>
	{
	}

	/// <summary>
	/// Interface for running "units of work" against the database.
	/// </summary>
	/// <typeparam name="T">Type of entity.</typeparam>
	public interface IRepository<T> : IRepositoryBase<IDatabaseContext<T>> where T : class, IDatabaseObject
	{
	}
}
