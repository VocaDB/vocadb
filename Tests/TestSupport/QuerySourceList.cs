#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.TestSupport
{
	public class QuerySourceList : IDatabaseContext
	{
		// Objects added (but not yet committed) during this transaction
		private readonly List<IDatabaseObject> _added = new();
		// Objects that were committed
		private readonly List<IDatabaseObject> _committed = new();
		private readonly Dictionary<Type, IList> _entities;

		public QuerySourceList()
		{
			_entities = new Dictionary<Type, IList>();
		}

		public int AbortedTransactionCount { get; private set; }
		public int CommittedTransactionCount { get; private set; }

		public void Add<TEntity>(TEntity entity) where TEntity : class, IDatabaseObject
		{
			_added.Add(entity);
			List<TEntity>().Add(entity);
		}

		public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDatabaseObject
		{
			_added.AddRange(entities);
			List<TEntity>().AddRange(entities);
		}

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			_added.Clear();
			_committed.Clear();

			void CommitTransaction()
			{
				CommittedTransactionCount++;
				_committed.AddRange(_added);
				_added.Clear();
			}

			void RollbackTransaction()
			{
				AbortedTransactionCount++;
				_added.Clear();
			}

			return new FakeTransaction(CommitTransaction, RollbackTransaction);
		}

		public void Dispose()
		{
		}

		public IAuditLogger AuditLogger => new FakeAuditLogger();

		public void Flush()
		{
		}

		/// <summary>
		/// Tests that an object was saved to database during an active transaction and the transaction was committed.
		/// </summary>
		public bool IsCommitted<TEntity>(TEntity entity) where TEntity : class, IDatabaseObject
			=> _committed.Contains(entity);

		public IDatabaseContext<T2> OfType<T2>() where T2 : class, IDatabaseObject => new ListDatabaseContext<T2>(this);

		public List<TEntity> List<TEntity>() where TEntity : class, IDatabaseObject
		{
			var t = typeof(TEntity);

			if (!_entities.ContainsKey(t))
				_entities.Add(t, new List<TEntity>());

			return (List<TEntity>)_entities[t];
		}

		public T Load<T>(object id) where T : class, IDatabaseObject
		{
			if (!typeof(IEntryWithIntId).IsAssignableFrom(typeof(T)) || !(id is int))
			{
				throw new NotSupportedException("Only supported for IEntryWithIntId and integer Ids");
			}

			var entity = List<T>().FirstOrDefault(e => ((IEntryWithIntId)e).Id == (int)id);
			return entity;
		}

		public IQueryable<T> Query<T>() where T : class, IDatabaseObject
		{
			var t = typeof(T);

			if (_entities.ContainsKey(t))
				return ((List<T>)_entities[t]).AsQueryable();
			else
				return (new T[] { }).AsQueryable();
		}

		/// <summary>
		/// Clears added and committed objects
		/// </summary>
		public void Reset()
		{
			_added.Clear();
			_committed.Clear();
			CommittedTransactionCount = AbortedTransactionCount = 0;
		}
	}
}
