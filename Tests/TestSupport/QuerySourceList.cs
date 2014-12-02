using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Search;

namespace VocaDb.Tests.TestSupport {

	public class QuerySourceList : IQuerySource {

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
