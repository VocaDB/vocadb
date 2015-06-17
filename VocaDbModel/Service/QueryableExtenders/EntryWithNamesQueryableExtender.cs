using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class EntryWithNamesQueryableExtender {

		public static IOrderedQueryable<T> OrderByEntryName<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference) 
			where T : IEntryWithNames {

			return FindHelpers.AddNameOrder(criteria, languagePreference);

		}

		public static IQueryable<EntryIdAndName> SelectIdAndName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference)
			where T: class, IEntryWithNames {

			switch (languagePreference) {
				case ContentLanguagePreference.English:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.English, Id = a.Id });
				case ContentLanguagePreference.Romaji:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Romaji, Id = a.Id });
				case ContentLanguagePreference.Japanese:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Japanese, Id = a.Id });
				default:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Japanese, Id = a.Id });
			}

		}

		public static IQueryable<EntryBaseContract> SelectEntryBase<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference, EntryType entryType)
			where T: class, IEntryWithNames {

			switch (languagePreference) {
				case ContentLanguagePreference.English:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.English, Id = a.Id, EntryType = entryType });
				case ContentLanguagePreference.Romaji:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Romaji, Id = a.Id, EntryType = entryType });
				case ContentLanguagePreference.Japanese:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Japanese, Id = a.Id, EntryType = entryType });
				default:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Japanese, Id = a.Id, EntryType = entryType });
			}

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
		public static IQueryable<TEntry> WhereHasNameGeneric<TEntry, TName>(this IQueryable<TEntry> query, SearchTextQuery textQuery) where TEntry : IEntryWithNames<TName> where TName : LocalizedStringWithId {

			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => m.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					foreach (var word in words.Take(FindHelpers.MaxSearchWords)) {
						query = query.Where(q => q.Names.Names.Any(n => n.Value.Contains(word)));
					}

					return query;

			}

			return query;

		}

	}
}
