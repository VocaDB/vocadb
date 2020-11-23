using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumWebLink : GenericWebLink<Album>
	{
		public AlbumWebLink() { }

		public AlbumWebLink(Album album, string description, string url, WebLinkCategory category)
			: base(album, description, url, category) { }
	}
}
