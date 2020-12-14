#nullable disable

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.TestSupport
{
	/// <summary>
	/// Fake in-memory repository for testing.
	/// </summary>
	/// <typeparam name="T">Type of entities this repository contains.</typeparam>
	public class FakeRepository<T> : IRepository<T>
		where T : class, IDatabaseObject
	{
		protected readonly QuerySourceList querySource;

		public FakeRepository()
		{
			querySource = new QuerySourceList();
		}

		public FakeRepository(params T[] items)
		{
			querySource = new QuerySourceList();
			Save(items);
		}

		public FakeRepository(QuerySourceList querySource)
		{
			this.querySource = querySource;
		}

		public int AbortedTransactionCount => querySource.AbortedTransactionCount;
		public int CommittedTransactionCount => querySource.CommittedTransactionCount;

		/// <summary>
		/// Adds a list of entities to the repository without performing any additional persisting logic.
		/// For example, Ids will not be generated this way.
		/// </summary>
		/// <typeparam name="TEntity">Type of entities to be added.</typeparam>
		/// <param name="entities">List of entities to be added. Cannot be null.</param>
		public void Add<TEntity>(params TEntity[] entities) where TEntity : class, IDatabaseObject
		{
			querySource.AddRange(entities);
		}

		public bool Contains<TEntity>(TEntity entity) where TEntity : class, IDatabaseObject
			=> querySource.List<TEntity>().Contains(entity);

		public int Count<TEntity>() where TEntity : class, IDatabaseObject
			=> querySource.List<TEntity>().Count;

		public virtual ListDatabaseContext<T> CreateContext()
		{
			return new ListDatabaseContext<T>(querySource);
		}

		public TResult HandleQuery<TResult>(Func<IDatabaseContext<T>, TResult> func, string failMsg = "Unexpected database error")
		{
			return func(CreateContext());
		}

		public Task<TResult> HandleQueryAsync<TResult>(Func<IDatabaseContext<T>, Task<TResult>> func, string failMsg = "Unexpected database error")
		{
			return func(CreateContext());
		}

		public void HandleTransaction(Action<IDatabaseContext<T>> func, string failMsg = "Unexpected database error")
		{
			func(CreateContext());
		}

		public TResult HandleTransaction<TResult>(Func<IDatabaseContext<T>, TResult> func, string failMsg = "Unexpected database error")
		{
			return func(CreateContext());
		}

		public Task<TResult> HandleTransactionAsync<TResult>(Func<IDatabaseContext<T>, Task<TResult>> func, string failMsg = "Unexpected database error")
		{
			return func(CreateContext());
		}

		public Task HandleTransactionAsync(Func<IDatabaseContext<T>, Task> func, string failMsg = "Unexpected database error")
		{
			return func(CreateContext());
		}

		/// <summary>
		/// Tests that an object was saved to database during an active transaction and the transaction was committed.
		/// </summary>
		public bool IsCommitted<T2>(T2 entity) where T2 : class, IDatabaseObject
			=> querySource.IsCommitted(entity);

		public List<TEntity> List<TEntity>() where TEntity : class, IDatabaseObject
			=> querySource.List<TEntity>();

		public T Load(int id) => HandleQuery(ctx => ctx.Load(id));

		public T Load(object id) => HandleQuery(ctx => ctx.Load(id));

		public T2 Load<T2>(object id) where T2 : class, IDatabaseObject => OfType<T2>().Load(id);

		public FakeRepository<T2> OfType<T2>() where T2 : class, IDatabaseObject
		{
			return new FakeRepository<T2>(querySource);
		}

		/// <summary>
		/// Save the entity into the repository using the repository's own Save method.
		/// Usually this means an Id will be assigned for the entity, if it's not persisted.
		/// </summary>
		/// <typeparam name="T2">Type of entity to be saved.</typeparam>
		/// <param name="objs">Entity to be saved. Cannot be null.</param>
		public void Save<T2>(params T2[] objs) where T2 : class, IDatabaseObject
		{
			foreach (var obj in objs)
				CreateContext().Save(obj);
		}

		public void Save<T2>(ICollection<T2> objs) where T2 : class, IDatabaseObject
		{
			foreach (var obj in objs)
				CreateContext().Save(obj);
		}

		/// <summary>
		/// Save the entity into the repository using the repository's own Save method.
		/// Usually this means an Id will be assigned for the entity, if it's not persisted.
		/// </summary>
		/// <typeparam name="T2">Type of entity to be saved.</typeparam>
		/// <param name="obj">Entity to be saved. Cannot be null.</param>
		public T2 Save<T2>(T2 obj) where T2 : class, IDatabaseObject
		{
			CreateContext().Save(obj);
			return obj;
		}

		public Song Save(Song song)
		{
			var ctx = CreateContext();

			ctx.Save(song);

			SaveNames(song);

			return song;
		}

		public void SaveNames<TName>(params IEntryWithNames<TName>[] entries)
			where TName : LocalizedStringWithId
		{
			foreach (var name in entries.SelectMany(e => e.Names.Names))
				Save(name);
		}

		public TEntry SaveWithNames<TEntry, TName>(TEntry entry)
			where TEntry : class, IEntryWithNames<TName>
			where TName : LocalizedStringWithId
		{
			CreateContext().Save(entry);
			SaveNames(entry);
			return entry;
		}
	}

	public class ListDatabaseContext<T> : IDatabaseContext<T>
		where T : class, IDatabaseObject
	{
		private static readonly bool isEntityWithId = typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)) || typeof(IEntryWithLongId).IsAssignableFrom(typeof(T));

		protected bool IsEntityWithId => isEntityWithId;

		// Get next Id
		private void AssignNewId(T obj)
		{
			switch (obj)
			{
				case IEntryWithIntId entityInt when entityInt.Id == 0:
					entityInt.Id = (Query().Any() ? Query().Max(o => ((IEntryWithIntId)o).Id) + 1 : 1);
					break;

				case IEntryWithLongId entityLong when entityLong.Id == 0:
					entityLong.Id = (Query().Any() ? Query().Max(o => ((IEntryWithLongId)o).Id) + 1 : 1);
					break;
			}
		}

		/// <summary>
		/// Tests whether an entity matches an Id.
		/// Generally only <see cref="GetId"/> needs to be overridden.
		/// </summary>
		/// <param name="entity">Entity to be tested. Cannot be null.</param>
		/// <param name="id">Id to be tested. Cannot be null.</param>
		/// <returns>True if the Id matches the entity, otherwise false.</returns>
		protected virtual bool IdEquals(T entity, object id)
		{
			if (IsEntityWithId && id is int)
				return ((IEntryWithIntId)entity).Id == (int)id;
			else
				return GetId(entity).Equals(id);
		}

		/// <summary>
		/// Gets the Id for an entity.
		/// If the entity is derived from <see cref="IEntryWithIntId"/> the integer
		/// Id will be used. Otherwise, the Id property will be attempted to be read
		/// by reflection.
		/// 
		/// This needs to be overridden for types that use composite Ids
		/// or if the Id property is named something else besides "Id".
		/// </summary>
		/// <param name="entity">Entity to be tested. Cannot be null.</param>
		/// <returns>Entity Id. Cannot be null (Id can never be null)</returns>
		protected virtual object GetId(T entity)
		{
			switch (entity)
			{
				case IEntryWithIntId id:
					return id.Id;
				case IEntryWithLongId longId:
					return longId.Id;
			}

			if (typeof(T).GetProperty("Id") == null)
				throw new NotSupportedException("Id property not found. You need to override GetId method for this repository.");

			dynamic dyn = entity;
			return dyn.Id;
		}

		protected readonly QuerySourceList querySource;

		public ListDatabaseContext(QuerySourceList querySource)
		{
			this.querySource = querySource;
		}

		public IAuditLogger AuditLogger => new FakeAuditLogger();

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel) => querySource.BeginTransaction(isolationLevel);

		public void Delete(T entity) => querySource.List<T>().Remove(entity);

		public Task DeleteAsync(T entity)
		{
			Delete(entity);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
		}

		public void Flush()
		{
		}

		public T Get(object id)
		{
			var list = querySource.List<T>();
			return list.FirstOrDefault(i => IdEquals(i, id));
		}

		public virtual T Load(object id)
		{
			var list = querySource.List<T>();

			if (list.All(i => !IdEquals(i, id)))
				throw new InvalidOperationException(string.Format("Entity of type {0} with Id {1} not found", typeof(T), id));

			return list.First(i => IdEquals(i, id));
		}

		public Task<T> LoadAsync(object id) => Task.FromResult(Load(id));

		public virtual IDatabaseContext<T2> OfType<T2>() where T2 : class, IDatabaseObject
			=> new ListDatabaseContext<T2>(querySource);

		public IQueryable<T> Query() => querySource.Query<T>();

		public IQueryable<T2> Query<T2>() where T2 : class, IDatabaseObject => OfType<T2>().Query();

		public T Save(T obj)
		{
			if (IsEntityWithId)
			{
				AssignNewId(obj);
			}

			querySource.Add(obj);
			return obj;
		}

		public Task<T> SaveAsync(T obj) => Task.FromResult(Save(obj));

		public virtual void Update(T obj)
		{
			var existing = Load(GetId(obj));
			Delete(existing);   // Replace existing
			Save(obj);
		}

		public Task UpdateAsync(T obj)
		{
			Update(obj);
			return Task.CompletedTask;
		}
	}
}
