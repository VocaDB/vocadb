using System;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.Helpers {

	public static class SongQueryableExtender {

		public static IQueryable<Song> AddOrder(this IQueryable<Song> criteria, SongSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case SongSortRule.Name:
					return criteria.AddNameOrder(languagePreference);
				case SongSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case SongSortRule.FavoritedTimes:
					return criteria.OrderByDescending(a => a.FavoritedTimes);
				case SongSortRule.RatingScore:
					return criteria.OrderByDescending(a => a.RatingScore);
			}

			return criteria;

		}

		public static IQueryable<Song> OrderBy(this IQueryable<Song> criteria, SongSortRule sortRule, ContentLanguagePreference languagePreference) {
			return AddOrder(criteria, sortRule, languagePreference);
		}

		public static IQueryable<Song> WhereArtistHasTag(this IQueryable<Song> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.AllArtists.Any(a => a.Artist.Tags.Usages.Any(u => u.Tag.Name == tagName)));

		}

		public static IQueryable<Song> WhereArtistHasType(this IQueryable<Song> query, ArtistType artistType) {

			if (artistType == ArtistType.Unknown)
				return query;

			return query.Where(s => s.AllArtists.Any(a => a.Artist.ArtistType == artistType));

		}

		public static IQueryable<Song> WhereDraftsOnly(this IQueryable<Song> query, bool draftsOnly) {

			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);

		}

		/*
		/// <summary>
		/// Filters a song query by a list of artist Ids.
		/// </summary>
		public static IQueryable<Song> WhereHasArtist(this IQueryable<Song> query, int[] artistIds) {

			if (artistIds == null || artistIds.Length == 0)
				return query;

			if (artistIds.Length == 1)
				return WhereHasArtist(query, artistIds.First());

			// TODO: should change to AND
			return query.Where(s => s.AllArtists.Any(a => artistIds.Contains(a.Artist.Id)));

		}*/

		/// <summary>
		/// Filters a song query by a single artist Id.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="artistId">ID of the artist being filtered. If 0, no filtering is done.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Song> WhereHasArtist(this IQueryable<Song> query, int artistId, bool childVoicebanks) {

			if (artistId == 0)
				return query;

			if (!childVoicebanks)
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId));
			else
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId || a.Artist.BaseVoicebank.Id == artistId));

		}

		public static IQueryable<Song> WhereHasArtistParticipationStatus(this IQueryable<Song> query, 
			int artistId, ArtistAlbumParticipationStatus participation, 
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

				// Note: producers may not have child voicebanks
				switch (participation) {
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(al => al.AllArtists.Any(a => a.Artist.Id == artistId && !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Song.ArtistString.Default != various));
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(al => al.AllArtists.Any(a => a.Artist.Id == artistId && (a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Song.ArtistString.Default == various)));
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

		public static IQueryable<Song> WhereHasLyrics(this IQueryable<Song> query, ContentLanguageSelection[] languages) {
			
			if (languages == null || !languages.Any())
				return query;

			if (languages.Length == 1) {
				var lang = languages.First();
				return query.Where(s => s.Lyrics.Any(l => l.Language == lang));				
			} else {

				var allLanguages = EnumVal<ContentLanguageSelection>.Values.All(languages.Contains);

				if (allLanguages) {
					// Has lyrics in any language
					return query.Where(s => s.Lyrics.Any());									
				} else {
					return query.Where(s => s.Lyrics.Any(l => languages.Contains(l.Language)));									
				}

			}

		} 

		/// <summary>
		/// Filters a song query by a name query.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Song> WhereHasName(this IQueryable<Song> query, SearchTextQuery textQuery) {

			return query.WhereHasNameGeneric<Song, SongName>(textQuery);

		}

		/// <summary>
		/// Filters a song link query by a name query.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		/// <remarks>
		/// Note: this code should be optimized after switching to EF.
		/// Cannot be optimized as is for NH.
		/// </remarks>
		public static IQueryable<T> WhereChildHasName<T>(this IQueryable<T> query, SearchTextQuery textQuery) where T : ISongLink {

			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Song.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => m.Song.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Song.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					switch (words.Length) {
						case 1:
							query = query.Where(q => q.Song.Names.Names.Any(n => n.Value.Contains(words[0])));
							break;
						case 2:
							query = query.Where(q => 
								q.Song.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[1]))
							);
							break;
						case 3:
							query = query.Where(q => 
								q.Song.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[2]))
							);
							break;
						case 4:
							query = query.Where(q => 
								q.Song.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[3]))
							);
							break;
						case 5:
							query = query.Where(q => 
								q.Song.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[4]))
							);
							break;
						case 6:
							query = query.Where(q => 
								q.Song.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[4]))
								&& q.Song.Names.Names.Any(n => n.Value.Contains(words[5]))
							);
							break;
					}
					return query;

			}

			return query;

		}

		public static IQueryable<Song> WhereHasNicoId(this IQueryable<Song> query, string nicoId) {

			if (string.IsNullOrEmpty(nicoId))
				return query;

			return query.Where(s => s.NicoId == nicoId);

		}

		public static IQueryable<Song> WhereHasTag(this IQueryable<Song> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Tags.Usages.Any(a => a.Tag.Name == tagName));

		}

		public static IQueryable<Song> WhereHasType(this IQueryable<Song> query, SongType[] songTypes) {

			if (!songTypes.Any())
				return query;

			return query.Where(m => songTypes.Contains(m.SongType));

		}

		public static IQueryable<Song> WhereIdNotIn(this IQueryable<Song> query, int[] ignoreIds) {

			if (ignoreIds == null || !ignoreIds.Any())
				return query;

			return query.Where(s => !ignoreIds.Contains(s.Id));

		} 

		public static IQueryable<Song> WhereInUserCollection(this IQueryable<Song> query, int userId) {

			if (userId == 0)
				return query;

			query = query.Where(s => s.UserFavorites.Any(a => a.User.Id == userId));

			return query;

		}

	}
}
