using System.Linq;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Model.Service.Helpers {

	public static class TagQueryableExtender {

		public static IQueryable<Tag> AddTagNameFilter(this IQueryable<Tag> query, TagSearchTextQuery textQuery) {

			return WhereHasName(query, textQuery);

		}

		public static IQueryable<Tag> WhereHasName(this IQueryable<Tag> query, TagSearchTextQuery textQuery) {

			return FindHelpers.AddTagNameFilter(query, textQuery);

		}

		public static IQueryable<Tag> WhereAllowAliases(this IQueryable<Tag> query, bool allowAliases = true) {

			if (allowAliases)
				return query;

			return query.Where(t => t.AliasedTo == null);

		}

		public static IQueryable<Tag> WhereHasCategoryName(this IQueryable<Tag> query, string categoryName) {

			if (string.IsNullOrEmpty(categoryName))
				return query;

			return query.Where(t => t.CategoryName == categoryName);

		}

	}
}
