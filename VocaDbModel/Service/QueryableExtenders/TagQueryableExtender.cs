using System.Linq;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class TagQueryableExtender {

		public static IQueryable<Tag> OrderBy(this IQueryable<Tag> query, TagSortRule sortRule) {

			switch (sortRule) {
				case TagSortRule.Name:
					return query.OrderBy(t => t.EnglishName);
				case TagSortRule.UsageCount:
					return query
						.OrderByDescending(t => t.AllAlbumTagUsages.Count + t.AllArtistTagUsages.Count + t.AllSongTagUsages.Count)
						.ThenBy(t => t.EnglishName);
			}

			return query;

		}

		public static IQueryable<Tag> WhereAllowAliases(this IQueryable<Tag> query, bool allowAliases = true) {

			if (allowAliases)
				return query;

			return query.Where(t => t.AliasedTo == null);

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

		public static IQueryable<Tag> WhereHasName(this IQueryable<Tag> query, TagSearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			var name = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.EnglishName == name);

				case NameMatchMode.StartsWith:
					return query.Where(m => m.EnglishName.StartsWith(name));

				default:
					return query.Where(m => m.EnglishName.Contains(name));
			}

		}

	}

	public enum TagSortRule {
		Nothing,
		Name,
		UsageCount
	}

}
