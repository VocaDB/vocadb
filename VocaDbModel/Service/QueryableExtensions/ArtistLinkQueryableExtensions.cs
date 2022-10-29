using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class ArtistLinkQueryableExtensions
	{
		public static IQueryable<T> OrderBy<T>(
			this IQueryable<T> criteria, ArtistSortRule sortRule, ContentLanguagePreference languagePreference) where T : IArtistLink => sortRule switch
		{
			ArtistSortRule.Name => criteria.OrderByName(languagePreference),
			ArtistSortRule.AdditionDate => criteria.OrderByDescending(a => a.Artist.CreateDate),
			ArtistSortRule.AdditionDateAsc => criteria.OrderBy(a => a.Artist.CreateDate),
			ArtistSortRule.ReleaseDate => criteria.OrderBy(a => a.Artist.ReleaseDate.DateTime),
			ArtistSortRule.SongCount => criteria.OrderByDescending(a => a.Artist.AllSongs.Count()),
			ArtistSortRule.SongRating => criteria.OrderByDescending(a => a.Artist.AllSongs.Where(s => !s.Song.Deleted).Sum(s => s.Song.RatingScore)),
			ArtistSortRule.FollowerCount => criteria.OrderByDescending(a => a.Artist.Users.Count),
			_ => criteria,
		};

		public static IOrderedQueryable<T> OrderByName<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IArtistLink => languagePreference switch
		{
			ContentLanguagePreference.Japanese => criteria.OrderBy(e => e.Artist.Names.SortNames.Japanese),
			ContentLanguagePreference.English => criteria.OrderBy(e => e.Artist.Names.SortNames.English),
			_ => criteria.OrderBy(e => e.Artist.Names.SortNames.Romaji),
		};

		public static IQueryable<T> WhereArtistHasName<T>(this IQueryable<T> query, ArtistSearchTextQuery? textQuery) where T : IArtistLink
		{
			if (textQuery == null || textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			return textQuery.MatchMode switch
			{
				NameMatchMode.Exact => query.Where(m => m.Artist.Names.Names.Any(n => n.Value == nameFilter)),
				NameMatchMode.Partial => query.Where(m => m.Artist.Names.Names.Any(n => n.Value.Contains(nameFilter))),
				NameMatchMode.StartsWith => query.Where(m => m.Artist.Names.Names.Any(n => n.Value.StartsWith(nameFilter))),
				NameMatchMode.Words => textQuery.Words.Take(FindHelpers.MaxSearchWords).Aggregate(query, (q, word) => q.Where(link => link.Artist.Names.Names.Any(n => n.Value.Contains(word)))),
				_ => query,
			};
		}

		public static IQueryable<T> WhereArtistHasType<T>(this IQueryable<T> query, ArtistType artistType) where T : IArtistLink
		{
			if (artistType == ArtistType.Unknown)
				return query;

			return query.Where(m => m.Artist.ArtistType == artistType);
		}

		public static IQueryable<T> WhereArtistHasType<T>(this IQueryable<T> query, ArtistType[] artistTypes) where T : IArtistLink
		{
			if (!artistTypes.Any())
				return query;

			return query.Where(m => artistTypes.Contains(m.Artist.ArtistType));
		}

		public static IQueryable<T> WhereArtistHasTag<T>(this IQueryable<T> query, int tagId)
			where T : IArtistLink
		{
			if (tagId == 0)
				return query;

			return query.Where(a => a.Artist.Tags.Usages.Any(t => t.Tag.Id == tagId));
		}

		public static IQueryable<T> WhereArtistHasTags<T>(this IQueryable<T> query, int[]? tagIds)
		 where T : IArtistLink
		{
			if (tagIds == null || !tagIds.Any())
				return query;

			return tagIds.Aggregate(query, WhereArtistHasTag);
		}
	}
}
