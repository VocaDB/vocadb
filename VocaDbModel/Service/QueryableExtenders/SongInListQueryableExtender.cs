using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders
{
	public static class SongInListQueryableExtender
	{
		private static Expression<Func<SongInList, bool>> GetNotesExpression(SearchTextQuery textQuery)
		{
			var query = textQuery.Query;

			switch (textQuery.MatchMode)
			{
				case NameMatchMode.Exact:
					return s => s.Notes != null && s.Notes == query;
				case NameMatchMode.Partial:
					return s => s.Notes != null && s.Notes.Contains(query);
				case NameMatchMode.StartsWith:
					return s => s.Notes != null && s.Notes.StartsWith(query);
				case NameMatchMode.Words:
					// Note: NHibernate does not support All
					return textQuery.Words.Aggregate(PredicateBuilder.True<SongInList>(), (nameExp, name) => nameExp.And(song => song.Notes != null && song.Notes.Contains(name)));
			}

			return m => true;
		}

		public static IQueryable<SongInList> OrderBy(this IQueryable<SongInList> query, SongSortRule? sortRule, ContentLanguagePreference languagePreference)
		{
			if (sortRule == null)
				return query.OrderBy(s => s.Order);

			return SongLinkQueryableExtender.OrderBy(query, sortRule.Value, languagePreference);
		}

		public static IQueryable<SongList> WhereHasTags(this IQueryable<SongList> query, int[] tagId, bool childTags = false)
		{
			return query.WhereHasTags<SongList, SongListTagUsage>(tagId, childTags);
		}

		public static IQueryable<SongInList> WhereSongHasName(this IQueryable<SongInList> query, SearchTextQuery textQuery, bool includeDescription)
		{
			if (textQuery.IsEmpty)
				return query;

			var expression = SongLinkQueryableExtender.GetChildHasNameExpression<SongInList>(textQuery);

			if (includeDescription)
			{
				expression = expression.Or(GetNotesExpression(textQuery));
			}

			return query.Where(expression);
		}
	}
}
