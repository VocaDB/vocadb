using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Songs {

	public class SongWebLink : GenericWebLink<Song> {

		public SongWebLink() { }

		public SongWebLink(Song song, string description, string url, WebLinkCategory category)
			: base(song, description, url, category) {}

	}

}
