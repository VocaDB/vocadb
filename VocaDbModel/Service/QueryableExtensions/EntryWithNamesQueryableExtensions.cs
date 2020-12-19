#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class EntryWithNamesQueryableExtensions
	{
		public static IOrderedQueryable<T> OrderByEntryName<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference)
			where T : IEntryWithNames
		{
			return FindHelpers.AddNameOrder(criteria, languagePreference);
		}

		public static IOrderedQueryable<T> ThenByEntryName<T>(this IOrderedQueryable<T> criteria, ContentLanguagePreference languagePreference)
			where T : IEntryWithNames
		{
			return FindHelpers.AddNameOrder(criteria, languagePreference);
		}

		public static IQueryable<EntryIdAndName> SelectIdAndName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference)
			where T : class, IEntryWithNames => languagePreference switch
		{
			ContentLanguagePreference.English => query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.English, Id = a.Id }),
			ContentLanguagePreference.Romaji => query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Romaji, Id = a.Id }),
			ContentLanguagePreference.Japanese => query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Japanese, Id = a.Id }),
			_ => query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Japanese, Id = a.Id }),
		};

		public static IQueryable<EntryBaseContract> SelectEntryBase<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference, EntryType entryType)
			where T : class, IEntryWithNames => languagePreference switch
		{
			ContentLanguagePreference.English => query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.English, Id = a.Id, EntryType = entryType }),
			ContentLanguagePreference.Romaji => query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Romaji, Id = a.Id, EntryType = entryType }),
			ContentLanguagePreference.Japanese => query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Japanese, Id = a.Id, EntryType = entryType }),
			_ => query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Japanese, Id = a.Id, EntryType = entryType }),
		};

		/// <summary>
		/// Filters query by one or more tag names.
		/// The tag has to match at least one of the names.
		/// For empty list of names (or null) nothing is matched.
		/// The name has to be exact match (case insensitive).
		/// </summary>
		/// <param name="query">Query to be filtered. Cannot be null.</param>
		/// <param name="names">List of names to filter by. Can be null or empty, but in that case no tags will be matched.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<TEntry> WhereHasNameGeneric<TEntry, TName>(this IQueryable<TEntry> query, IEnumerable<SearchTextQuery> names)
			where TEntry : IEntryWithNames<TName> where TName : LocalizedStringWithId
		{
			names ??= new SearchTextQuery[0];

			var predicate = names.Aggregate(PredicateBuilder.False<TEntry>(), (nameExp, name) => nameExp.Or(WhereHasNameExpression<TEntry, TName>(name)));
			return query.Where(predicate);
		}

		public static Expression<Func<TEntry, bool>> WhereHasNameExpression<TEntry, TName>(SearchTextQuery textQuery) where TEntry
			: IEntryWithNames<TName> where TName : LocalizedStringWithId
		{
			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode)
			{
				case NameMatchMode.Exact:
					return m => m.Names.Names.Any(n => n.Value == nameFilter);

				case NameMatchMode.Partial:
					return m => m.Names.Names.Any(n => n.Value.Contains(nameFilter));

				case NameMatchMode.StartsWith:
					return m => m.Names.Names.Any(n => n.Value.StartsWith(nameFilter));

				case NameMatchMode.Words:
					var predicate = textQuery.Words.Aggregate(PredicateBuilder.True<TEntry>(), (nameExp, name) => nameExp.And(q => q.Names.Names.Any(n => n.Value.Contains(name))));
					return predicate;
			}

			return m => true;
		}

		/// <summary>
		/// Filters an entry query by entry name.
		/// </summary>
		/// <param name="query">Entry query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		/// <typeparam name="TEntry">Entry type.</typeparam>
		/// <typeparam name="TName">Entry name type.</typeparam>
		public static IQueryable<TEntry> WhereHasNameGeneric<TEntry, TName>(this IQueryable<TEntry> query, SearchTextQuery textQuery) where TEntry
			: IEntryWithNames<TName> where TName : LocalizedStringWithId
		{
			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;
			var expression = WhereHasNameExpression<TEntry, TName>(textQuery);

			return query.Where(expression);
		}
	}
}
