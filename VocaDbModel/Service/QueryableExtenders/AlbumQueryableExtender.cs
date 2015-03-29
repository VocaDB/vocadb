using System;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class AlbumQueryableExtender {

		public static IQueryable<Album> OrderByReleaseDate(this IQueryable<Album> query) {
			
			return query
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day);

		} 

		public static IQueryable<Album> OrderBy(this IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference) {
			
			switch (sortRule) {
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.CollectionCount:
					return criteria.OrderByDescending(a => a.UserCollections.Count);
				case AlbumSortRule.ReleaseDate:
					return criteria
						.WhereHasReleaseDate()
						.OrderByReleaseDate();
				case AlbumSortRule.ReleaseDateWithNulls:
					return criteria.OrderByReleaseDate();
				case AlbumSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case AlbumSortRule.RatingAverage:
					return criteria.OrderByDescending(a => a.RatingAverageInt)
						.ThenByDescending(a => a.RatingCount);
				case AlbumSortRule.RatingTotal:
					return criteria.OrderByDescending(a => a.RatingTotal)
						.ThenByDescending(a => a.RatingAverageInt);
				case AlbumSortRule.NameThenReleaseDate:
					return FindHelpers.AddNameOrder(criteria, languagePreference)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Day);
			}

			return criteria;

		}

		public static IQueryable<Album> WhereDraftsOnly(this IQueryable<Album> query, bool draftsOnly) {

			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);

		}

		public static IQueryable<Album> WhereHasArtistParticipationStatus(
			this IQueryable<Album> query, int artistId, ArtistAlbumParticipationStatus participation, 
			bool childVoicebanks,
			Func<int, Artist> artistGetter) {

			if (artistId == 0)
				return query;

			if (participation == ArtistAlbumParticipationStatus.Everything)
				return query.WhereHasArtist(artistId, childVoicebanks);

			var artist = artistGetter(artistId);
			var musicProducerTypes = new[] {ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup};

			if (musicProducerTypes.Contains(artist.ArtistType)) {

				var various = Model.Helpers.ArtistHelper.VariousArtists;
				var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

				switch (participation) {
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(al => al.AllArtists.Any(a => a.Artist.Id == artistId && !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Album.ArtistString.Default != various));
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(al => al.AllArtists.Any(a => a.Artist.Id == artistId && (a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Album.ArtistString.Default == various)));
					default:
						return query;
				}

			} else {

				switch (participation) {
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(al => al.AllArtists.Any(a => (a.Artist.Id == artistId || (childVoicebanks && a.Artist.BaseVoicebank.Id == artistId)) && !a.IsSupport));
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(al => al.AllArtists.Any(a => (a.Artist.Id == artistId || (childVoicebanks && a.Artist.BaseVoicebank.Id == artistId)) && a.IsSupport));
					default:
						return query;
				}
				
			}

		}

		/// <summary>
		/// Filters a album query by a single artist Id.
		/// </summary>
		/// <param name="query">Album query. Cannot be null.</param>
		/// <param name="artistId">ID of the artist being filtered. If 0, no filtering is done.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Album> WhereHasArtist(this IQueryable<Album> query, int artistId, bool childVoicebanks) {

			if (artistId == 0)
				return query;

			if (!childVoicebanks)
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId));
			else
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId || a.Artist.BaseVoicebank.Id == artistId));

		}

		public static IQueryable<Album> WhereHasBarcode(this IQueryable<Album> query, string barcode) {

			if (string.IsNullOrEmpty(barcode))
				return query;

			return query.Where(a => a.Identifiers.Any(i => i.Value == barcode));

		}

		// TODO: should be combined with common name query somehow, but NH is making it hard
		/// <summary>
		/// Filters an artist query by a name query.
		/// </summary>
		/// <param name="query">Artist query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Album> WhereHasName(this IQueryable<Album> query, SearchTextQuery textQuery, bool allowCatNum = false) {

			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => 
						(allowCatNum && m.OriginalRelease.CatNum != null && m.OriginalRelease.CatNum.Contains(nameFilter)) ||
						m.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => 
						(allowCatNum && m.OriginalRelease.CatNum != null && m.OriginalRelease.CatNum.StartsWith(nameFilter)) ||
						m.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					switch (words.Length) {
						case 1:
							query = query.Where(q => 
								(allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter)) ||
								q.Names.Names.Any(n => n.Value.Contains(words[0])));
							break;
						case 2:
							query = query.Where(q => 
								(allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter)) ||
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
							);
							break;
						case 3:
							query = query.Where(q => 
								(allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter)) ||
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
							);
							break;
						case 4:
							query = query.Where(q => 
								(allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter)) ||
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
							);
							break;
						case 5:
							query = query.Where(q => 
								(allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter)) ||
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
							);
							break;
						case 6:
							query = query.Where(q => 
								(allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter)) ||
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[5]))
							);
							break;
					}
					return query;

			}

			return query;

		}

		public static IQueryable<Album> WhereHasReleaseDate(this IQueryable<Album> criteria) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);

		}

		public static IQueryable<Album> WhereHasTag(this IQueryable<Album> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Tags.Usages.Any(a => a.Tag.Name == tagName));

		}

		public static IQueryable<Album> WhereHasType(this IQueryable<Album> query, DiscType albumType) {

			if (albumType == DiscType.Unknown)
				return query;

			return query.Where(m => m.DiscType == albumType);

		}

		public static IQueryable<Album> WhereIsDeleted(this IQueryable<Album> query, bool deleted) {
			
			return query.Where(m => m.Deleted == deleted);

		} 

	}
}
