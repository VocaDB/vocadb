using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class AlbumLinkQueryableExtender {

		public static IQueryable<T> WhereAlbumHasArtistWithType<T>(this IQueryable<T> query, ArtistType artistType)
			where T : IAlbumLink {

			if (artistType == ArtistType.Unknown)
				return query;

			return query.Where(s => s.Album.AllArtists.Any(a => !a.IsSupport && a.Artist.ArtistType == artistType));

		}

		public static IQueryable<T> WhereAlbumHasTag<T>(this IQueryable<T> query, string tagName)
			where T : IAlbumLink {
			
			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Album.Tags.Usages.Any(t => t.Tag.Names.SortNames.English == tagName || t.Tag.Names.SortNames.Romaji == tagName || t.Tag.Names.SortNames.Japanese == tagName));

		}

		public static IQueryable<T> WhereAlbumHasTag<T>(this IQueryable<T> query, int tagId)
			where T : IAlbumLink {

			if (tagId == 0)
				return query;

			return query.Where(s => s.Album.Tags.Usages.Any(t => t.Tag.Id == tagId));

		}

		public static IQueryable<T> WhereAlbumHasType<T>(this IQueryable<T> query, DiscType albumType) where T : IAlbumLink {

			if (albumType == DiscType.Unknown)
				return query;

			return query.Where(m => m.Album.DiscType == albumType);

		}

		public static IQueryable<T> WhereAlbumHasLinkWithCategory<T>(this IQueryable<T> query, WebLinkCategory category) 
			where T : IAlbumLink {

			return query.Where(m => m.Album.WebLinks.Any(l => l.Category == category));

		}

		public static IQueryable<T> WhereAlbumMatchFilter<T>(this IQueryable<T> query, AdvancedSearchFilter filter)
			where T : IAlbumLink {

			if (filter == null)
				return query;

			switch (filter.FilterType) {
				case AdvancedFilterType.ArtistType: {
					var param = EnumVal<ArtistType>.Parse(filter.Param);
					return WhereAlbumHasArtistWithType(query, param);
				}
				case AdvancedFilterType.NoCoverPicture: {
					return query.Where(a => a.Album.CoverPictureMime == null || a.Album.CoverPictureMime == string.Empty);
				}
				case AdvancedFilterType.HasStoreLink: {
					return query.WhereAlbumHasLinkWithCategory(WebLinkCategory.Commercial);
				}
				case AdvancedFilterType.HasTracks: {
					return query.Where(a => filter.Negate != a.Album.AllSongs.Any(s => s.Song == null || !s.Song.Deleted));
				}
			}

			return query;

		}

		public static IQueryable<T> WhereAlbumMatchFilters<T>(this IQueryable<T> query, IEnumerable<AdvancedSearchFilter> filters)
			where T : IAlbumLink {

			return filters?.Aggregate(query, WhereAlbumMatchFilter) ?? query;

		}

	}

}
