#nullable disable

using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class SongIconLinkViewModel
	{
		public SongIconLinkViewModel(SongContract song, int? albumId = null)
		{
			Song = song;
			AlbumId = albumId;
		}

		public SongContract Song { get; set; }

		public int? AlbumId { get; set; }
	}
}