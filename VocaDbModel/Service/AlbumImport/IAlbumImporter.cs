namespace VocaDb.Model.Service.AlbumImport
{

	public interface IAlbumImporter
	{

		string ServiceName { get; }

		AlbumImportResult ImportOne(string url);

		bool IsValidFor(string url);

	}

}
