using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.QueryableExtenders
{
	public static class GenericQueryableExtender
	{
		public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
		{
			return direction == SortDirection.Ascending ?
				query.OrderBy(keySelector) :
				query.OrderByDescending(keySelector);
		}

		public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
		{
			return direction == SortDirection.Ascending ?
				query.ThenBy(keySelector) :
				query.ThenByDescending(keySelector);
		}

		/// <summary>
		/// Filters query for paging.
		/// </summary>
		/// <typeparam name="T">Queried root entity.</typeparam>
		/// <param name="query">Query. Cannot be null.</param>
		/// <param name="paging">Paging properties. Can be null.</param>
		/// <param name="allowEmpty">Allow empty result if MaxResults is less than 1</param>
		/// <returns>Query which is filtered for paging.</returns>
		public static IQueryable<T> Paged<T>(this IQueryable<T> query, PagingProperties paging, bool allowEmpty = false)
		{
			if (paging == null)
				return query;

			if (allowEmpty && paging.MaxEntries <= 0)
				return new T[0].AsQueryable();

			var start = Math.Max(paging.Start, 0);
			var maxEntries = Math.Max(paging.MaxEntries, 1); // Take for database must be >= 1

			return query.Skip(start).Take(maxEntries);
		}

		// http://blog.jukkahyv.com/2016/04/09/transform-object-to-another-type-in-a-linq-query/
		public static IQueryable<TResult> SelectObject<TSource, TResult>(this IQueryable<TSource> query)
		{
			var t1 = typeof(TSource);
			var t2 = typeof(TResult);
			var properties = t2.GetProperties().Where(p => p.CanWrite && t1.GetProperty(p.Name)?.PropertyType == p.PropertyType).ToArray();
			var param = Expression.Parameter(typeof(TSource), "p");
			var memberBindings = properties.Select(targetProperty =>
				Expression.Bind(targetProperty, Expression.Property(param, t1.GetProperty(targetProperty.Name)))); // Prop = p.Prop
			var memberInit = Expression.MemberInit(Expression.New(typeof(TResult)), memberBindings); // new T2 { Prop = p.Prop, ... }

			return query.Select(Expression.Lambda<Func<TSource, TResult>>(memberInit, param));
		}

		public static Task<bool> VdbAnyAsync<TSource>(this IQueryable<TSource> source)
		{
			if (source.Provider is INhQueryProvider)
			{
				return source.AnyAsync();
			}

			return Task.FromResult(source.Any());
		}

		public static Task<int> VdbCountAsync<TSource>(this IQueryable<TSource> source)
		{
			if (source.Provider is INhQueryProvider)
			{
				return source.CountAsync();
			}

			return Task.FromResult(source.Count());
		}

		public static Task<TSource> VdbFirstOrDefaultAsync<TSource>(this IQueryable<TSource> source)
		{
			if (source.Provider is INhQueryProvider)
			{
				return source.FirstOrDefaultAsync();
			}

			return Task.FromResult(source.FirstOrDefault());
		}

		/// <summary>
		/// Executes the query and returns its result as <see cref="IList{T}"/>.
		/// To be used instead of the NHibernate extension method to make the query testable.
		/// Calls NHibernate's method when supported, otherwise non-async version.
		/// </summary>
		public static Task<List<TSource>> VdbToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Note: ToListAsync only supports INhQueryProvider
			if (source.Provider is INhQueryProvider)
			{
				return source.ToListAsync();
			}

			return Task.FromResult(source.ToList());
		}

		public static IQueryable<T> WhereEntryTypeIsIncluded<T>(this IQueryable<T> source, EntryTypes? entryTypes, EntryType entryType)
		{
			if (entryTypes == null || entryTypes.Value.HasFlag((EntryTypes)entryType))
				return source;
			return new List<T>().AsQueryable();
		}

		public static IQueryable<T> WhereIdIn<T>(this IQueryable<T> query, IEnumerable<int> ids) where T : IEntryWithIntId
			=> query.Where(e => ids.Contains(e.Id));
	}

	public enum SortDirection
	{
		Ascending,
		Descending
	}
}
