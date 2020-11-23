using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtenders
{
	public static class ArtistForAlbumQueryableExtender
	{
		public static IQueryable<ArtistForAlbum> WhereHasArtistParticipationStatus(this IQueryable<ArtistForAlbum> query, Artist artist, ArtistAlbumParticipationStatus participation)
		{
			if (participation == ArtistAlbumParticipationStatus.Everything || artist == null)
				return query;

			var musicProducerTypes = new[] { ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup };

			if (musicProducerTypes.Contains(artist.ArtistType))
			{
				var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

				switch (participation)
				{
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(a => !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Album.ArtistString.Default != ArtistHelper.VariousArtists);
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(a => a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Album.ArtistString.Default == ArtistHelper.VariousArtists);
					default:
						return query;
				}
			}
			else
			{
				switch (participation)
				{
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(a => !a.IsSupport);
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(a => a.IsSupport);
					default:
						return query;
				}
			}
		}
	}
}
