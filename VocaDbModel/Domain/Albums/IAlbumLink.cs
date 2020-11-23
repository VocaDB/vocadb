namespace VocaDb.Model.Domain.Albums
{
	/// <summary>
	/// Entities linked to <see cref="Albums.Album"/>.
	/// Usually this means child entities owned by the album.
	/// </summary>
	public interface IAlbumLink
	{
		Album Album { get; }
	}
}
