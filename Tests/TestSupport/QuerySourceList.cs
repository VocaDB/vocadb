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
		private readonly List<IDatabaseObject> added = new List<IDatabaseObject>();
		// Objects that were committed
		private readonly List<IDatabaseObject> committed = new List<IDatabaseObject>();
		private readonly Dictionary<Type, IList> entities;

		public QuerySourceList()
		{
			entities = new Dictionary<Type, IList>();
		}

		public int AbortedTransactionCount { get; private set; }
		public int CommittedTransactionCount { get; private set; }

		public void Add<TEntity>(TEntity entity) where TEntity : class, IDatabaseObject
		{
			added.Add(entity);
			List<TEntity>().Add(entity);
		}

		public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDatabaseObject
		{
			added.AddRange(entities);
			List<TEntity>().AddRange(entities);
		}

		public IMinimalTransaction BeginTransaction(IsolationLevel isolationLevel)
		{

			added.Clear();
			committed.Clear();

			void CommitTransaction()
			{
				CommittedTransactionCount++;
				committed.AddRange(added);
				added.Clear();
			}

			void RollbackTransaction()
			{
				AbortedTransactionCount++;
				added.Clear();
			}

			return new FakeTransaction(CommitTransaction, RollbackTransaction);

		}

		public void Dispose()
		{

		}

		public IAuditLogger AuditLogger
		{
			get
			{
				return new FakeAuditLogger();
			}
		}

		public void Flush()
		{

		}

		/// <summary>
		/// Tests that an object was saved to database during an active transaction and the transaction was committed.
		/// </summary>
		public bool IsCommitted<TEntity>(TEntity entity) where TEntity : class, IDatabaseObject
			=> committed.Contains(entity);

		public IDatabaseContext<T2> OfType<T2>() where T2 : class, IDatabaseObject => new ListDatabaseContext<T2>(this);

		public List<TEntity> List<TEntity>() where TEntity : class, IDatabaseObject
		{

			var t = typeof(TEntity);

			if (!entities.ContainsKey(t))
				entities.Add(t, new List<TEntity>());

			return (List<TEntity>)entities[t];

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

			if (entities.ContainsKey(t))
				return ((List<T>)entities[t]).AsQueryable();
			else
				return (new T[] { }).AsQueryable();

		}

		/// <summary>
		/// Clears added and committed objects
		/// </summary>
		public void Reset()
		{
			added.Clear();
			committed.Clear();
			CommittedTransactionCount = AbortedTransactionCount = 0;
		}

	}
}
