using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VocaDb.Model.Service {

	/// <summary>
	/// <see cref="IQueryable{T}"/> which may or may not be ordered.
	/// </summary>
	public interface IMaybeOrderedQueryable<out T> : IQueryable<T> { }

	public class MaybeOrderedQueryable<T> : IMaybeOrderedQueryable<T> {

		public MaybeOrderedQueryable(IQueryable<T> query, bool isOrdered) {
			this.query = query;
			IsOrdered = isOrdered;
		}

		public MaybeOrderedQueryable(IOrderedQueryable<T> query) {
			this.query = query;
			IsOrdered = true;
		}

		private readonly IQueryable<T> query;

		public Expression Expression => query.Expression;
		public Type ElementType => query.ElementType;
		public bool IsOrdered { get; }
		public IQueryProvider Provider => query.Provider;
		public IEnumerator<T> GetEnumerator() => query.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => query.GetEnumerator();

	}

	public static class MaybeOrderedQueryable {
		public static IMaybeOrderedQueryable<T> Create<T>(IQueryable<T> query, bool isOrdered = false) => new MaybeOrderedQueryable<T>(query, isOrdered);
		public static IMaybeOrderedQueryable<T> Create<T>(IOrderedQueryable<T> query) => new MaybeOrderedQueryable<T>(query);
	}

}
