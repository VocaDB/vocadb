using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class AlbumSearchSort
	{
		private static IQueryable<Album> AddReleaseRestriction(IQueryable<Album> criteria)
		{
			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);
		}

		public static IQueryable<Album> AddOrder(IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference)
		{
			switch (sortRule)
			{
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.ReleaseDate:
					return AddReleaseRestriction(criteria)
						.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day);
				case AlbumSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case AlbumSortRule.RatingAverage:
					return criteria.OrderByDescending(a => a.RatingAverageInt)
						.ThenByDescending(a => a.RatingCount);
				case AlbumSortRule.NameThenReleaseDate:
					return FindHelpers.AddNameOrder(criteria, languagePreference)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Day);
			}

			return criteria;
		}
	}
}
