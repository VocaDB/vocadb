using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.AlbumImport {

	public class AlbumImporters {

		private readonly IAlbumImporter[] importers;

		public AlbumImporters()
			: this(new WebPictureDownloader()) {}

		public AlbumImporters(IPictureDownloader pictureDownloader) {
			importers = new IAlbumImporter[] { new KarenTAlbumImporter(pictureDownloader) };
		}

		public IAlbumImporter FindImporter(VocaDbUrl url) {
			return importers.FirstOrDefault(i => i.IsValidFor(url));
		}

		public AlbumImportResult ImportOne(VocaDbUrl url) {

			var importer = FindImporter(url);

			if (importer == null)
				return new AlbumImportResult {Message = "URL not recognized"};

			return importer.ImportOne(url);

		}

	}
}
