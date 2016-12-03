using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.QueryableExtenders {

	/// <summary>
	/// Extensions methods for queryable of <see cref="ArtistForSong"/>.
	/// </summary>
	public static class ArtistForSongQueryableExtender {

		/// <summary>
		/// Filters out songs that aren't "main" songs for a particular artist type.
		/// 
		/// At the moment this just filters out PVs for music producers.
		/// Used by the "recent songs" list on artist page.
		/// </summary>
		/// <param name="query">Query. Cannot be null.</param>
		/// <param name="artistType">Artist type.</param>
		/// <returns>Filtered query.</returns>
		public static IQueryable<ArtistForSong> WhereIsMainSong(this IQueryable<ArtistForSong> query, ArtistType artistType) {
			
			if (artistType == ArtistType.Producer) {

				return query
					.Where(q => q.Song.SongType != SongType.MusicPV || (q.Roles & ArtistRoles.Animator) != ArtistRoles.Default)
					.WhereDoesNotHaveRole(ArtistRoles.VocalDataProvider);

			}

			return query;

		}

	}
}
