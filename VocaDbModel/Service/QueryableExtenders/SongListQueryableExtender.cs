using System.Linq;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class SongListQueryableExtender {

		public static IQueryable<SongList> OrderBy(this IQueryable<SongList> query, SongListSortRule sortRule) {

			switch (sortRule) {
				case SongListSortRule.Date:
					return query
						.OrderByDescending(r => r.EventDate)
						.ThenBy(r => r.Name);
				case SongListSortRule.CreateDate:
					return query.OrderByDescending(r => r.CreateDate);
				case SongListSortRule.Name:
					return query.OrderBy(r => r.Name);
			}

			return query;

		}

		public static IQueryable<SongList> WhereHasFeaturedCategory(this IQueryable<SongList> query, SongListFeaturedCategory? featuredCategory, bool allowNothing) {

			if (!featuredCategory.HasValue)
				return allowNothing ? query : query.Where(s => s.FeaturedCategory != SongListFeaturedCategory.Nothing);

			return query.Where(s => s.FeaturedCategory == featuredCategory.Value);

		}

		public static IQueryable<SongList> WhereHasName(this IQueryable<SongList> query, SearchTextQuery textQuery) {

			if (textQuery == null || textQuery.IsEmpty)
				return query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.StartsWith:
					return query.Where(u => u.Name.StartsWith(textQuery.Query));
				case NameMatchMode.Partial:
					return query.Where(u => u.Name.Contains(textQuery.Query));
				case NameMatchMode.Exact:
					return query.Where(u => u.Name == textQuery.Query);
				case NameMatchMode.Words:
					return textQuery.Words
						.Take(FindHelpers.MaxSearchWords)
						.Aggregate(query, (q, word) => q.Where(list => list.Name.Contains(word)));
			}

			return query;

		}

	}

	public enum SongListSortRule {
		
		None,

		Name,

		Date,

		CreateDate

	}

}
