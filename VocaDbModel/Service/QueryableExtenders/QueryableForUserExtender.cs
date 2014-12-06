using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class QueryableForUserExtender {

		public static IQueryable<User> WhereHasName(this IQueryable<User> query, SearchTextQuery textQuery) {

			if (textQuery.IsEmpty)
				return query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.StartsWith:
					return query.Where(u => u.Name.StartsWith(textQuery.Query));
				case NameMatchMode.Partial:
					return query.Where(u => u.Name.Contains(textQuery.Query));
				case NameMatchMode.Exact:
					return query.Where(u => u.Name == textQuery.Query);

			}

			return query;

		}


	}

}
