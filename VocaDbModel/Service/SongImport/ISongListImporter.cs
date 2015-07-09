using System.Linq;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Service.Rankings;

namespace VocaDb.Model.Service.SongImport {

	public interface ISongListImporter {

		PartialImportedSongs GetSongs(string url, string nextPageToken, bool parseAll);
			
		ImportedSongListContract Parse(string url, bool parseAll);

		bool MatchUrl(string url);

	}

	public class SongListImporters {

		private readonly ISongListImporter[] importers = {
			new NNDWVRParser(),
			new YoutubePlaylistImporter()
		};

		public ImportedSongListContract GetSongs(string url, bool parseAll) {
			
			var importer = importers.FirstOrDefault(i => i.MatchUrl(url));

			if (importer == null)
				throw new InvalidFeedException(string.Format("URL {0} is not recognized. Check the URL and try again", url));

			return importer.Parse(url, parseAll);

		}

	}

}
