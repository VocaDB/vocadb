using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.TestSupport {

	/// <summary>
	/// Fake in-memory repository for testing.
	/// </summary>
	/// <typeparam name="T">Type of entities this repository contains.</typeparam>
	public class FakeRepository<T> : IRepository<T> {

		protected readonly QuerySourceList querySource;

		public FakeRepository() {
			querySource = new QuerySourceList();
		}
 
		public FakeRepository(params T[] items) {
			querySource = new QuerySourceList();
			Save(items);
		} 

		public FakeRepository(QuerySourceList querySource) {
			this.querySource = querySource;
		}

		/// <summary>
		/// Adds a list of entities to the repository without performing any additional persisting logic.
		/// For example, Ids will not be generated this way.
		/// </summary>
		/// <typeparam name="TEntity">Type of entities to be added.</typeparam>
		/// <param name="entities">List of entities to be added. Cannot be null.</param>
		public void Add<TEntity>(params TEntity[] entities) {
			querySource.AddRange(entities);
		}

		public bool Contains<TEntity>(TEntity entity) {
			return querySource.List<TEntity>().Contains(entity);
		}

		public virtual ListDatabaseContext<T> CreateContext() {
			return new ListDatabaseContext<T>(querySource);
		}

		public TResult HandleQuery<TResult>(Func<IDatabaseContext<T>, TResult> func, string failMsg = "Unexpected database error") {
			return func(CreateContext());
		}

		public void HandleTransaction(Action<IDatabaseContext<T>> func, string failMsg = "Unexpected database error") {
			func(CreateContext());
		}

		public TResult HandleTransaction<TResult>(Func<IDatabaseContext<T>, TResult> func, string failMsg = "Unexpected database error") {
			return func(CreateContext());
		}

		public List<TEntity> List<TEntity>() {
			return querySource.List<TEntity>();
		}

		public T Load(object id) {
			return HandleQuery(ctx => ctx.Load(id));
		}

		public T2 Load<T2>(object id) {
			return OfType<T2>().Load(id);
		}

		public FakeRepository<T2> OfType<T2>() {
			return new FakeRepository<T2>(querySource);
		}

		/// <summary>
		/// Save the entity into the repository using the repository's own Save method.
		/// Usually this means an Id will be assigned for the entity, if it's not persisted.
		/// </summary>
		/// <typeparam name="T2">Type of entity to be saved.</typeparam>
		/// <param name="objs">Entity to be saved. Cannot be null.</param>
		public void Save<T2>(params T2[] objs) {
			foreach (var obj in objs)
				CreateContext().Save(obj);
		}

		/// <summary>
		/// Save the entity into the repository using the repository's own Save method.
		/// Usually this means an Id will be assigned for the entity, if it's not persisted.
		/// </summary>
		/// <typeparam name="T2">Type of entity to be saved.</typeparam>
		/// <param name="obj">Entity to be saved. Cannot be null.</param>
		public T2 Save<T2>(T2 obj) {
			CreateContext().Save(obj);
			return obj;
		}

		public Song Save(Song song) {
			
			var ctx = CreateContext();

			ctx.Save(song);

			SaveNames(song);

			return song;

		}

		public void SaveNames<TName>(params IEntryWithNames<TName>[] entries) 
			where TName : LocalizedStringWithId {

			foreach (var name in entries.SelectMany(e => e.Names.Names))
				Save(name);

		}

	}

	public class ListDatabaseContext<T> : IDatabaseContext<T> {

		private static readonly bool isEntityWithId = typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)) || typeof(IEntryWithLongId).IsAssignableFrom(typeof(T));

		protected bool IsEntityWithId {
			get { return isEntityWithId; }
		}

		// Get next Id
		private void AssignNewId(T obj) {

			var entityInt = obj as IEntryWithIntId;

			if (entityInt != null && entityInt.Id == 0) {
				entityInt.Id = (Query().Any() ? Query().Max(o => ((IEntryWithIntId)o).Id) + 1 : 1);
			}

			var entityLong = obj as IEntryWithLongId;
			if (entityLong != null && entityLong.Id == 0) {
				entityLong.Id = (Query().Any() ? Query().Max(o => ((IEntryWithLongId)o).Id) + 1 : 1);
			}

		}

		/// <summary>
		/// Tests whether an entity matches an Id.
		/// Generally only <see cref="GetId"/> needs to be overridden.
		/// </summary>
		/// <param name="entity">Entity to be tested. Cannot be null.</param>
		/// <param name="id">Id to be tested. Cannot be null.</param>
		/// <returns>True if the Id matches the entity, otherwise false.</returns>
		protected virtual bool IdEquals(T entity, object id) {
			
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
		protected virtual object GetId(T entity) {
			
			if (entity is IEntryWithIntId)
				return ((IEntryWithIntId)entity).Id;

			if (entity is IEntryWithLongId)
				return ((IEntryWithLongId)entity).Id;

			if (typeof(T).GetProperty("Id") == null)
				throw new NotSupportedException("Id property not found. You need to override GetId method for this repository.");

			dynamic dyn = entity;
			return dyn.Id;

		}

		protected readonly QuerySourceList querySource;

		public ListDatabaseContext(QuerySourceList querySource) {
			this.querySource = querySource;
		}

		public IAuditLogger AuditLogger {
			get {
				return new FakeAuditLogger();
			}
		}

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel) {
			return new FakeTransaction();
		}

		public void Delete(T entity) {
			querySource.List<T>().Remove(entity);
		}

		public void Dispose() {
			
		}

		public void Flush() {
			
		}

		public T Get(object id) {
							
			var list = querySource.List<T>();
			return list.FirstOrDefault(i => IdEquals(i, id));

		}

		public virtual T Load(object id) {

			var list = querySource.List<T>();

			if (list.All(i => !IdEquals(i, id)))
				throw new InvalidOperationException(string.Format("Entity of type {0} with Id {1} not found", typeof(T), id));

			return list.First(i => IdEquals(i, id));

		}

		public virtual IDatabaseContext<T2> OfType<T2>() {
			return new ListDatabaseContext<T2>(querySource);
		}

		public IQueryable<T> Query() {
			return querySource.Query<T>();
		}

		public IQueryable<T2> Query<T2>() {
			return OfType<T2>().Query();
		}

		public void Save(T obj) {

			if (IsEntityWithId) {
				AssignNewId(obj);
			}

			querySource.Add(obj);

		}

		public virtual void Update(T obj) {

			var existing = Load(GetId(obj));
			Delete(existing);	// Replace existing
			Save(obj);

		}

	}

}
