#nullable disable

using System.Linq;

namespace VocaDb.Model.DataContracts.Albums
{
	public class RelatedAlbumsContract
	{
		public bool Any => ArtistMatches.Any() || LikeMatches.Any() || TagMatches.Any();

		public AlbumContract[] ArtistMatches { get; init; }

		public AlbumContract[] LikeMatches { get; init; }

		public AlbumContract[] TagMatches { get; init; }
	}
}
