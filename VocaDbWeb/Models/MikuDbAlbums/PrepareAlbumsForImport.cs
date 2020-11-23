using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Web.Models.MikuDbAlbums
{
	public class PrepareAlbumsForImport
	{
		public PrepareAlbumsForImport() { }

		public PrepareAlbumsForImport(InspectedAlbum album)
		{
			Album = album;
		}

		public InspectedAlbum Album { get; set; }
	}
}