#nullable disable

using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.Helpers
{
	public static class FindHelpers
	{
		public const int MaxSearchWords = 10;

		private static Expression<Func<T, string>> OrderByExpression<T>(ContentLanguagePreference languagePreference) where T : IEntryWithNames
		{
			switch (languagePreference)
			{
				case ContentLanguagePreference.Japanese:
					return e => e.Names.SortNames.Japanese;
				case ContentLanguagePreference.English:
					return e => e.Names.SortNames.English;
				case ContentLanguagePreference.Romaji:
					return e => e.Names.SortNames.Romaji;
				default:
					// Note: the Default name field is not mapped to database so we're selecting it here dynamically. There is some small performance penalty.
					return e => e.Names.SortNames.DefaultLanguage == ContentLanguageSelection.English ? e.Names.SortNames.English : (e.Names.SortNames.DefaultLanguage == ContentLanguageSelection.Romaji ? e.Names.SortNames.Romaji : e.Names.SortNames.Japanese);
			}
		}

		public static IOrderedQueryable<T> AddNameOrder<T>(IQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IEntryWithNames
		{
			return criteria.OrderBy(OrderByExpression<T>(languagePreference));
		}

		public static IOrderedQueryable<T> AddNameOrder<T>(IOrderedQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IEntryWithNames
		{
			return criteria.ThenBy(OrderByExpression<T>(languagePreference));
		}

		/// <summary>
		/// Processes T-SQL wildcards, specifically the brackets "[]" and percent wildcard "%" from the search term.
		/// </summary>
		/// <param name="term">Search term, for example "alone [SNDI RMX]". Can be null or empty.</param>
		/// <returns>Cleaned (escaped) term, for example "alone [[]SNDI RMX]".</returns>
		/// <remarks>
		/// Because brackets are used for character group wildcards in T-SQL "like" queries, 
		/// searches such as "alone [SNDI RMX]" did not work.
		/// </remarks>
		public static string CleanTerm(string term)
		{
			if (string.IsNullOrEmpty(term))
				return term;

			return term.Replace("[", "[[]").Replace("%", "[%]");
		}

		private static bool ShouldEncodeSQLCharacters(NameMatchMode matchMode)
		{
			return matchMode == NameMatchMode.Partial || matchMode == NameMatchMode.StartsWith || matchMode == NameMatchMode.Words;
		}

		/// <summary>
		/// Gets match mode and query for search.
		/// 
		/// Handles short query terms and wildcard searches ("*" in the end).
		/// Will only be applied if the current match mode is Auto.
		/// </summary>
		/// <param name="query">Text query. Can be null or empty.</param>
		/// <param name="matchMode">Current match mode. If Auto, will be set if something else besides Auto.</param>
		/// <param name="defaultMode">Default match mode to be used for normal queries.</param>
		/// <returns>Text query. Wildcard characters are removed. Can be null or empty, if original query is.</returns>
		public static string GetMatchModeAndQueryForSearch(string query, ref NameMatchMode matchMode, NameMatchMode defaultMode = NameMatchMode.Words)
		{
			if (string.IsNullOrEmpty(query))
				return query;

			// Remove SQL wildcard characters from the query
			query = query.Trim();

			// If name match mode is already decided, there's nothing more to do
			if (matchMode != NameMatchMode.Auto)
			{
				if (ShouldEncodeSQLCharacters(matchMode))
				{
					query = CleanTerm(query);
				}
				return query;
			}

			if (query.Length > 1 && query.StartsWith("*"))
			{
				matchMode = NameMatchMode.Words;
				return CleanTerm(query).Substring(1);
			}

			if (query.Length > 1 && query.EndsWith("*"))
			{
				matchMode = NameMatchMode.StartsWith;
				return CleanTerm(query).Substring(0, query.Length - 1);
			}

			if (query.Length > 2 && query.StartsWith("\"") && query.EndsWith("\""))
			{
				matchMode = NameMatchMode.Exact;
				return query.Substring(1, query.Length - 2);
			}

			if (query.Length <= 2)
			{
				matchMode = NameMatchMode.StartsWith;
				return CleanTerm(query);
			}

			matchMode = defaultMode;

			if (ShouldEncodeSQLCharacters(matchMode))
			{
				query = CleanTerm(query);
			}

			return query;
		}

		public static string[] GetQueryWords(string query)
		{
			return query
				.Split(new[] { ' ' }, MaxSearchWords, StringSplitOptions.RemoveEmptyEntries)
				.Distinct()
				.ToArray();
		}
	}
}
