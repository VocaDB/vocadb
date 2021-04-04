using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class SongInListQueryableExtensions
	{
		private static Expression<Func<SongInList, bool>> GetNotesExpression(SearchTextQuery textQuery)
		{
			var query = textQuery.Query;

			return textQuery.MatchMode switch
			{
				NameMatchMode.Exact => s => s.Notes != null && s.Notes == query,
				NameMatchMode.Partial => s => s.Notes != null && s.Notes.Contains(query),
				NameMatchMode.StartsWith => s => s.Notes != null && s.Notes.StartsWith(query),
				// Note: NHibernate does not support All
				NameMatchMode.Words => textQuery.Words.Aggregate(PredicateBuilder.True<SongInList>(), (nameExp, name) => nameExp.And(song => song.Notes != null && song.Notes.Contains(name))),
				_ => m => true,
			};
		}

		public static IQueryable<SongInList> OrderBy(this IQueryable<SongInList> query, SongSortRule? sortRule, ContentLanguagePreference languagePreference)
		{
			if (sortRule == null)
				return query.OrderBy(s => s.Order);

			return SongLinkQueryableExtensions.OrderBy(query, sortRule.Value, languagePreference);
		}

		public static IQueryable<SongList> WhereHasTags(this IQueryable<SongList> query, int[]? tagId, bool childTags = false)
		{
			return query.WhereHasTags<SongList, SongListTagUsage>(tagId, childTags);
		}

		public static IQueryable<SongInList> WhereSongHasName(this IQueryable<SongInList> query, SearchTextQuery textQuery, bool includeDescription)
		{
			if (textQuery.IsEmpty)
				return query;

			var expression = SongLinkQueryableExtensions.GetChildHasNameExpression<SongInList>(textQuery);

			if (includeDescription)
			{
				expression = expression.Or(GetNotesExpression(textQuery));
			}

			return query.Where(expression);
		}
	}
}
