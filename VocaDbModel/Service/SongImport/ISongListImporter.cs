using System.Linq;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Service.Rankings;

namespace VocaDb.Model.Service.SongImport {

	public interface ISongListImporter {

		PartialImportedSongs GetSongs(string url, string nextPageToken, int maxResults, bool parseAll);
			
		ImportedSongListContract Parse(string url, bool parseAll);

		bool MatchUrl(string url);

	}

	public class SongListImporters {

		private readonly ISongListImporter[] importers = {
			new NicoNicoMyListParser(),
			new YoutubePlaylistImporter()
		};

		private ISongListImporter GetImporter(string url) {
			
			var importer = importers.FirstOrDefault(i => i.MatchUrl(url));

			if (importer == null)
				throw new InvalidFeedException(string.Format("URL {0} is not recognized. Check the URL and try again", url));

			return importer;

		}

		public PartialImportedSongs GetSongs(string url, string pageToken, int maxResults, bool parseAll) {
			
			return GetImporter(url).GetSongs(url, pageToken, maxResults, parseAll);

		}

		public ImportedSongListContract Parse(string url, bool parseAll) {
			
			return GetImporter(url).Parse(url, parseAll);

		}

	}

}
