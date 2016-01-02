using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.TestSupport {

	public class QuerySourceList : IDatabaseContext {

		private readonly Dictionary<Type, IList> entities;

		public QuerySourceList() {
			entities = new Dictionary<Type, IList>();
		}

		public void Add<TEntity>(TEntity entity) {
			List<TEntity>().Add(entity);
		}

		public void AddRange<TEntity>(params TEntity[] entities) {
			List<TEntity>().AddRange(entities);
		}

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel) {
			return new FakeTransaction();
        }

		public void Dispose() {
			
		}

		public IAuditLogger AuditLogger {
			get {
				return new FakeAuditLogger();
			}
		}

		public void Flush() {
			
		}

		public IDatabaseContext<T2> OfType<T2>() {
			return new ListDatabaseContext<T2>(this);
		}

		public List<TEntity> List<TEntity>() {

			var t = typeof(TEntity);

			if (!entities.ContainsKey(t))
				entities.Add(t, new List<TEntity>());

			return (List<TEntity>)entities[t];

		}

		public T Load<T>(object id) {

			if (!typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)) || !(id is int))
			{
				throw new NotSupportedException("Only supported for IEntryWithIntId and integer Ids");
			}

			var entity = List<T>().FirstOrDefault(e => ((IEntryWithIntId)e).Id == (int)id);
			return entity;

		}

		public IQueryable<T> Query<T>() {
		
			var t = typeof(T);

			if (entities.ContainsKey(t))
				return ((List<T>)entities[t]).AsQueryable();
			else
				return (new T[] { }).AsQueryable();

		}

	}
}
