using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Shared.Partials.Album {

	public class AlbumIconLinkViewModel {

		public AlbumIconLinkViewModel(AlbumContract album) {
			Album = album;
		}

		public AlbumContract Album { get; set; }

	}

}