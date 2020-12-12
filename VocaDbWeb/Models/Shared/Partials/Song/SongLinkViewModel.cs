using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class SongLinkViewModel
	{
		public SongLinkViewModel(SongContract song, int? albumId = null, bool tooltip = false)
		{
			Song = song;
			AlbumId = albumId;
			Tooltip = tooltip;
		}

		public SongContract Song { get; set; }

		public int? AlbumId { get; set; }

		public bool Tooltip { get; set; }
	}
}