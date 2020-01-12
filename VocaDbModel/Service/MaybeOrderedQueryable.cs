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

		public MaybeOrderedQueryable(IQueryable<T> query) {
			this.query = query;
		}

		private readonly IQueryable<T> query;

		public Expression Expression => query.Expression;
		public Type ElementType => query.ElementType;
		public IQueryProvider Provider => query.Provider;
		public IEnumerator<T> GetEnumerator() => query.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => query.GetEnumerator();

	}

}
