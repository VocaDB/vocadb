using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions;

public static class UserQueryableExtensions
{
	public static IQueryable<User> WhereHasName(this IQueryable<User> query, SearchTextQuery textQuery)
	{
		if (textQuery.IsEmpty)
			return query;

		return textQuery.MatchMode switch
		{
			NameMatchMode.StartsWith => query.Where(u => u.Name.StartsWith(textQuery.Query)),
			// Words search doesn't really make sense for usernames, so using partial matching
			NameMatchMode.Partial or NameMatchMode.Words => query.Where(u => u.Name.Contains(textQuery.Query)),
			NameMatchMode.Exact => query.Where(u => u.Name == textQuery.Query),
			_ => query,
		};
	}

	public static IQueryable<User> WhereIsActive(this IQueryable<User> query) => query.Where(u => u.Active);

	public static IQueryable<User> WhereKnowsLanguage(this IQueryable<User> query, string? langCode)
	{
		if (string.IsNullOrEmpty(langCode))
			return query;

		return query.Where(u => u.KnownLanguages.Any(l => l.CultureCode.CultureCode == langCode));
	}
}
