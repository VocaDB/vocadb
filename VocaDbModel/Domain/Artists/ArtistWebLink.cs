using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Artists
{

	public class ArtistWebLink : GenericWebLink<Artist>
	{

		public ArtistWebLink() { }

		public ArtistWebLink(Artist artist, string description, string url, WebLinkCategory category)
			: base(artist, description, url, category) { }

	}

}
