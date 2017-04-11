using System;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class TagQueryableExtender {

		public static IQueryable<Tag> OrderBy(this IQueryable<Tag> query, TagSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case TagSortRule.AdditionDate:
					return query.OrderByDescending(t => t.CreateDate);
				case TagSortRule.Name:
					return query.OrderByEntryName(languagePreference);
				case TagSortRule.UsageCount:
					return query.OrderByDescending(t => t.UsageCount);
			}

			return query;

		}

		public static IQueryable<Tag> OrderBy(
			this IQueryable<Tag> query, EntrySortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case EntrySortRule.Name:
					return query.OrderByEntryName(languagePreference);
				case EntrySortRule.AdditionDate:
					return query.OrderByDescending(a => a.CreateDate);
			}

			return query;

		}

		public static IQueryable<Tag> WhereAllowChildren(this IQueryable<Tag> query, bool allowChildren = true) {

			if (allowChildren)
				return query;

			return query.Where(t => t.Parent == null);

		}

		public static IQueryable<Tag> WhereHasCategoryName(this IQueryable<Tag> query, string categoryName) {

			return WhereHasCategoryName(query, SearchTextQuery.Create(categoryName, NameMatchMode.Exact));

		}

		public static IQueryable<Tag> WhereHasCategoryName(this IQueryable<Tag> query, SearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(t => t.CategoryName == textQuery.Query);
				case NameMatchMode.StartsWith:
					return query.Where(t => t.CategoryName.StartsWith(textQuery.Query));
				default:
					return query.Where(t => t.CategoryName.Contains(textQuery.Query));
			}

		}

		public static IQueryable<Tag> WhereHasName(this IQueryable<Tag> query, SearchTextQuery textQuery) {

			return query.WhereHasNameGeneric<Tag, TagName>(textQuery);

		}

		/// <summary>
		/// Filters query by one or more tag names.
		/// The tag has to match at least one of the names.
		/// For empty list of names (or null) nothing is matched.
		/// The name has to be exact match (case insensitive).
		/// </summary>
		/// <param name="query">Query to be filtered. Cannot be null.</param>
		/// <param name="names">List of names to filter by. Can be null or empty, but in that case no tags will be matched.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Tag> WhereHasName(this IQueryable<Tag> query, params string[] names) {

			names = names ?? new string[0];

			var predicate = names.Aggregate(PredicateBuilder.False<Tag>(), (nameExp, name) => nameExp.Or(q => q.Names.Names.Any(n => n.Value == name)));
			return query.Where(predicate);

		}

		public static IQueryable<Tag> WhereHasTarget(this IQueryable<Tag> query, TagTargetTypes target) {

			if (target == TagTargetTypes.All)
				return query;

			return query.Where(t => (t.Targets & target) == target);

		}

	}

	public enum TagSortRule {
		Nothing,
		Name,
		AdditionDate,
		UsageCount
	}

}
