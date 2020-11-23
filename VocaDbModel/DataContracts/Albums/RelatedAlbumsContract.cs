using System.Linq;

namespace VocaDb.Model.DataContracts.Albums
{
	public class RelatedAlbumsContract
	{
		public bool Any
		{
			get
			{
				return ArtistMatches.Any() || LikeMatches.Any() || TagMatches.Any();
			}
		}

		public AlbumContract[] ArtistMatches { get; set; }

		public AlbumContract[] LikeMatches { get; set; }

		public AlbumContract[] TagMatches { get; set; }
	}
}
