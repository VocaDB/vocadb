using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class EntryWithArtistsQueryableExtender {

		/// <summary>
		/// Filters a album query by a single artist Id.
		/// </summary>
		/// <param name="query">Album query. Cannot be null.</param>
		/// <param name="artistId">ID of the artist being filtered. If 0, no filtering is done.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<TEntry> WhereHasArtist<TEntry, TArtistLink>(this IQueryable<TEntry> query, int artistId, bool childVoicebanks)
			where TEntry : IEntryWithArtists<TArtistLink> where TArtistLink : IArtistLink {

			if (artistId == 0)
				return query;

			if (!childVoicebanks)
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId));
			else
				return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId || a.Artist.BaseVoicebank.Id == artistId));

		}

		public static IQueryable<TEntry> WhereHasArtistParticipationStatus<TEntry, TArtistLink>(
			IQueryable<TEntry> query, int artistId, ArtistAlbumParticipationStatus participation, 
			bool childVoicebanks,
			Func<int, Artist> artistGetter,
			Expression<Func<TEntry, bool>> mainEntriesExpression,
			Expression<Func<TEntry, bool>> collaborationsExpression)
			where TEntry : IEntryWithArtists<TArtistLink> where TArtistLink : IArtistWithSupport {

			if (artistId == 0)
				return query;

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

	}

}
