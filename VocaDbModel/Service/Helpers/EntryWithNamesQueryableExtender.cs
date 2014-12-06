using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.Helpers {

	public static class EntryWithNamesQueryableExtender {

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

					switch (words.Length) {
						case 1:
							query = query.Where(q => q.Names.Names.Any(n => n.Value.Contains(words[0])));
							break;
						case 2:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
							);
							break;
						case 3:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
							);
							break;
						case 4:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
							);
							break;
						case 5:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
							);
							break;
						case 6:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[5]))
							);
							break;
					}
					return query;

			}

			return query;

		}

	}
}
