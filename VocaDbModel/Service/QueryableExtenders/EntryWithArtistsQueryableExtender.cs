using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class EntryWithArtistsQueryableExtender {

		public static IQueryable<TEntry> WhereArtistHasType<TEntry, TArtistLink>(this IQueryable<TEntry> query, ArtistType artistType)
			where TEntry : IEntryWithArtistLinks<TArtistLink>
			where TArtistLink : IArtistWithSupport {

			if (artistType == ArtistType.Unknown)
				return query;

			return query.Where(s => s.AllArtists.Any(a => !a.IsSupport && a.Artist.ArtistType == artistType));

		}

		/// <summary>
		/// Filters an entry query by a single artist Id.
		/// </summary>
		/// <param name="query">Entry query. Cannot be null.</param>
		/// <param name="artistId">ID of the artist being filtered. If 0, no filtering is done.</param>
		/// <param name="childVoicebanks">Whether child voicebanks of possible voice synthesizer are included.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<TEntry> WhereHasArtist<TEntry, TArtistLink>(this IQueryable<TEntry> query, int artistId, bool childVoicebanks)
			where TEntry : IEntryWithArtistLinks<TArtistLink> 
			where TArtistLink : IArtistLink {

			if (artistId == 0)
				return query;

			if (!childVoicebanks)
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId));
			else
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId || a.Artist.BaseVoicebank.Id == artistId 
					|| a.Artist.BaseVoicebank.BaseVoicebank.Id == artistId || a.Artist.BaseVoicebank.BaseVoicebank.BaseVoicebank.Id == artistId));

		}

		/// <summary>
		/// Filters an entry query by a list of artist Ids.
		/// </summary>
		/// <param name="query">Entry query. Cannot be null.</param>
		/// <param name="artistIds">IDs of the artists being filtered. If null or empty, no filtering is done.</param>
		/// <param name="childVoicebanks">Whether child voicebanks of possible voice synthesizer are included.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<TEntry> WhereHasArtists<TEntry, TArtistLink>(this IQueryable<TEntry> query, EntryIdsCollection artistIds, bool childVoicebanks)
			where TEntry : IEntryWithArtistLinks<TArtistLink> where TArtistLink : IArtistLink {

			if (!artistIds.HasAny)
				return query;

			return artistIds.Ids.Aggregate(query, (current, artistId) => current.WhereHasArtist<TEntry, TArtistLink>(artistId, childVoicebanks));

		}

		public static IQueryable<TEntry> WhereHasArtistParticipationStatus<TEntry, TArtistLink>(
			ArtistParticipationQueryParams<TEntry, TArtistLink> queryParams)
			where TEntry : IEntryWithArtistLinks<TArtistLink> where TArtistLink : IArtistWithSupport {

			var query = queryParams.Query;
			var childVoicebanks = queryParams.ChildVoicebanks;

			if (!queryParams.ArtistIds.HasAny)
				return query;

			if (queryParams.ArtistIds.HasMultiple)
				return WhereHasArtists<TEntry, TArtistLink>(query, queryParams.ArtistIds, childVoicebanks);

			var participation = queryParams.Participation;
			var artistGetter = queryParams.ArtistGetter;
			var artistId = queryParams.ArtistIds.Primary;
			var mainEntriesExpression = queryParams.MainEntriesExpression;
			var collaborationsExpression = queryParams.CollaborationsExpression;

			if (participation == ArtistAlbumParticipationStatus.Everything)
				return WhereHasArtist<TEntry, TArtistLink>(query, artistId, childVoicebanks);

			var artist = artistGetter(artistId);
			var musicProducerTypes = new[] {ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup};

			if (musicProducerTypes.Contains(artist.ArtistType)) {

				switch (participation) {
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(mainEntriesExpression);
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(collaborationsExpression);
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

		public static IQueryable<TEntry> WhereHasArtists<TEntry, TArtistLink>(this IQueryable<TEntry> query, string[] artistNames)
			where TEntry : IEntryWithArtistLinks<TArtistLink> where TArtistLink : IArtistLink {

			if (artistNames == null || artistNames.Length == 0)
				return query;

			return artistNames.Aggregate(query, (current, artistName) => current.Where(e => e.AllArtists.Any(a => a.Artist.Names.Names.Any(n => n.Value == artistName))));

		}

	}

	public struct ArtistParticipationQueryParams<TEntry, TArtistLink> 
		where TEntry : IEntryWithArtistLinks<TArtistLink> where TArtistLink : IArtistWithSupport {
		
		public ArtistParticipationQueryParams(IQueryable<TEntry> query, EntryIdsCollection artistIds, ArtistAlbumParticipationStatus participation, 
			bool childVoicebanks,
			Func<int, Artist> artistGetter,
			Expression<Func<TEntry, bool>> mainEntriesExpression,
			Expression<Func<TEntry, bool>> collaborationsExpression)
			: this() {
			
			Query = query;
			ArtistIds = artistIds;
			Participation = participation;
			ChildVoicebanks = childVoicebanks;
			ArtistGetter = artistGetter;
			MainEntriesExpression = mainEntriesExpression;
			CollaborationsExpression = collaborationsExpression;

		}

		public IQueryable<TEntry> Query { get; private set; } 
		public EntryIdsCollection ArtistIds { get; private set; } 
		public ArtistAlbumParticipationStatus Participation { get; private set; }
		public bool ChildVoicebanks { get; private set; }
		public Func<int, Artist> ArtistGetter { get; private set; }
		public Expression<Func<TEntry, bool>> MainEntriesExpression { get; private set; }
		public Expression<Func<TEntry, bool>> CollaborationsExpression { get; private set; }

	}

}
