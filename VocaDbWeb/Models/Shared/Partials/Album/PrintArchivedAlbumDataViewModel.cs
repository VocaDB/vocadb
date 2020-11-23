using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Shared.Partials.Album
{

	public class PrintArchivedAlbumDataViewModel
	{

		public PrintArchivedAlbumDataViewModel(ComparedAlbumsContract comparedAlbums)
		{
			ComparedAlbums = comparedAlbums;
		}

		public ComparedAlbumsContract ComparedAlbums { get; set; }

	}

}