#nullable disable

using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class ArtistForAlbumQueryableExtensions
	{
		public static IQueryable<ArtistForAlbum> WhereHasArtistParticipationStatus(this IQueryable<ArtistForAlbum> query, Artist artist, ArtistAlbumParticipationStatus participation)
		{
			if (participation == ArtistAlbumParticipationStatus.Everything || artist == null)
				return query;

			var musicProducerTypes = new[] { ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup };

			if (musicProducerTypes.Contains(artist.ArtistType))
			{
				var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

				return participation switch
				{
					ArtistAlbumParticipationStatus.OnlyMainAlbums => query.Where(a => !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Album.ArtistString.Default != ArtistHelper.VariousArtists),
					ArtistAlbumParticipationStatus.OnlyCollaborations => query.Where(a => a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Album.ArtistString.Default == ArtistHelper.VariousArtists),
					_ => query,
				};
			}
			else
			{
				return participation switch
				{
					ArtistAlbumParticipationStatus.OnlyMainAlbums => query.Where(a => !a.IsSupport),
					ArtistAlbumParticipationStatus.OnlyCollaborations => query.Where(a => a.IsSupport),
					_ => query,
				};
			}
		}
	}
}
