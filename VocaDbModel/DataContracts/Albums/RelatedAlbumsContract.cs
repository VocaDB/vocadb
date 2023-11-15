#nullable disable


namespace VocaDb.Model.DataContracts.Albums;

public class RelatedAlbumsContract
{
	public bool Any => ArtistMatches.Any() || LikeMatches.Any() || TagMatches.Any();

	public AlbumForApiContract[] ArtistMatches { get; init; }

	public AlbumForApiContract[] LikeMatches { get; init; }

	public AlbumForApiContract[] TagMatches { get; init; }
}
