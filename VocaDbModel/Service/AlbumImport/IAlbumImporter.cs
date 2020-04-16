using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.AlbumImport {

	public interface IAlbumImporter {

		string ServiceName { get; }

		AlbumImportResult ImportOne(VocaDbUrl url);

		bool IsValidFor(VocaDbUrl url);

	}

}
