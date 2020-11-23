using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Model.Service.AlbumImport
{
	public class AlbumImportResult
	{
		public MikuDbAlbumContract AlbumContract { get; set; }

		/// <summary>
		/// Result message, such as a warning explaining why the album couldn't be imported.
		/// </summary>
		public string Message { get; set; }
	}
}
