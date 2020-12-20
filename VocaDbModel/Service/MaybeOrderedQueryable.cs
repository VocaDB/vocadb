#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VocaDb.Model.Service
{
	/// <summary>
	/// <see cref="IQueryable{T}"/> which may or may not be ordered.
	/// </summary>
	public interface IMaybeOrderedQueryable<out T> : IQueryable<T> { }

	public class MaybeOrderedQueryable<T> : IMaybeOrderedQueryable<T>
	{
		public MaybeOrderedQueryable(IQueryable<T> query, bool isOrdered)
		{
			_query = query;
			IsOrdered = isOrdered;
		}

		public MaybeOrderedQueryable(IOrderedQueryable<T> query)
		{
			_query = query;
			IsOrdered = true;
		}

		private readonly IQueryable<T> _query;

		public Expression Expression => _query.Expression;
		public Type ElementType => _query.ElementType;
		public bool IsOrdered { get; }
		public IQueryProvider Provider => _query.Provider;
		public IEnumerator<T> GetEnumerator() => _query.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _query.GetEnumerator();
	}

	public static class MaybeOrderedQueryable
	{
		public static IMaybeOrderedQueryable<T> Create<T>(IQueryable<T> query, bool isOrdered = false) => new MaybeOrderedQueryable<T>(query, isOrdered);
		public static IMaybeOrderedQueryable<T> Create<T>(IOrderedQueryable<T> query) => new MaybeOrderedQueryable<T>(query);
	}
}
