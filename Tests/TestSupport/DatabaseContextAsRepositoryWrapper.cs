#nullable disable

using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.TestSupport;

/// <summary>
/// Wraps <see cref="IDatabaseContext"/> as <see cref="IRepository"/>.
/// TODO: maybe remove this.
/// </summary>
public class DatabaseContextAsRepositoryWrapper : IRepository
{
	private readonly IDatabaseContext _dbContext;

	public DatabaseContextAsRepositoryWrapper(IDatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}

	public TResult HandleQuery<TResult>(Func<IDatabaseContext, TResult> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public Task<TResult> HandleQueryAsync<TResult>(Func<IDatabaseContext, Task<TResult>> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public void HandleTransaction(Action<IDatabaseContext> func, string failMsg = "Unexpected database error")
	{
		func(_dbContext);
	}

	public TResult HandleTransaction<TResult>(Func<IDatabaseContext, TResult> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public Task<TResult> HandleTransactionAsync<TResult>(Func<IDatabaseContext, Task<TResult>> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public Task HandleTransactionAsync(Func<IDatabaseContext, Task> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}
}

public class DatabaseContextAsRepositoryWrapper<TRepo> : IRepository<TRepo>
	where TRepo : class, IDatabaseObject
{
	private readonly IDatabaseContext<TRepo> _dbContext;

	public DatabaseContextAsRepositoryWrapper(IDatabaseContext<TRepo> dbContext)
	{
		_dbContext = dbContext;
	}

	public TResult HandleQuery<TResult>(Func<IDatabaseContext<TRepo>, TResult> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public Task<TResult> HandleQueryAsync<TResult>(Func<IDatabaseContext<TRepo>, Task<TResult>> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public void HandleTransaction(Action<IDatabaseContext<TRepo>> func, string failMsg = "Unexpected database error")
	{
		func(_dbContext);
	}

	public TResult HandleTransaction<TResult>(Func<IDatabaseContext<TRepo>, TResult> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public Task<TResult> HandleTransactionAsync<TResult>(Func<IDatabaseContext<TRepo>, Task<TResult>> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}

	public Task HandleTransactionAsync(Func<IDatabaseContext<TRepo>, Task> func, string failMsg = "Unexpected database error")
	{
		return func(_dbContext);
	}
}
