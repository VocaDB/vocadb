using System.Linq;
using VocaDb.Model.Domain.Songs;

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

	}

	public enum SongListSortRule {
		
		None,

		Name,

		Date,

		CreateDate

	}

}
