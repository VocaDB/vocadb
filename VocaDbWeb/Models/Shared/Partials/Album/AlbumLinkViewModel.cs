#nullable disable

using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Shared.Partials.Album
{
	public class AlbumLinkViewModel
	{
		public AlbumLinkViewModel(AlbumContract album)
		{
			Album = album;
		}

		public AlbumContract Album { get; set; }
	}
}