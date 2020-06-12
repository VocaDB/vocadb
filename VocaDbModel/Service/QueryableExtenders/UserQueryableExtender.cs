using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class UserQueryableExtender {

		public static IQueryable<User> WhereHasName(this IQueryable<User> query, SearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.StartsWith:
					return query.Where(u => u.Name.StartsWith(textQuery.Query));
				case NameMatchMode.Partial:
				case NameMatchMode.Words: // Words search doesn't really make sense for usernames, so using partial matching
					return query.Where(u => u.Name.Contains(textQuery.Query));
				case NameMatchMode.Exact:
					return query.Where(u => u.Name == textQuery.Query);

			}

			return query;

		}

		public static IQueryable<User> WhereIsActive(this IQueryable<User> query) => query.Where(u => u.Active);

		public static IQueryable<User> WhereKnowsLanguage(this IQueryable<User> query, string langCode) {

			if (string.IsNullOrEmpty(langCode))
				return query;

			return query.Where(u => u.KnownLanguages.Any(l => l.CultureCode.CultureCode == langCode));

		}

	}

}
