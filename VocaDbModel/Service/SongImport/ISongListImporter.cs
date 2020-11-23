using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.SongImport;

namespace VocaDb.Model.Service.SongImport
{

	public interface ISongListImporter
	{

		Task<PartialImportedSongs> GetSongsAsync(string url, string nextPageToken, int maxResults, bool parseAll);

		Task<ImportedSongListContract> ParseAsync(string url, bool parseAll);

		bool MatchUrl(string url);

	}

	public class SongListImporters
	{

		private readonly ISongListImporter[] importers = {
			new NicoNicoMyListParser(),
			new YoutubePlaylistImporter()
		};

		private ISongListImporter GetImporter(string url)
		{

			var importer = importers.FirstOrDefault(i => i.MatchUrl(url));

			if (importer == null)
				throw new UnableToImportException(string.Format("URL {0} is not recognized. Check the URL and try again", url));

			return importer;

		}

		public Task<PartialImportedSongs> GetSongs(string url, string pageToken, int maxResults, bool parseAll)
		{

			return GetImporter(url).GetSongsAsync(url, pageToken, maxResults, parseAll);

		}

		public Task<ImportedSongListContract> Parse(string url, bool parseAll)
		{

			return GetImporter(url).ParseAsync(url, parseAll);

		}

	}

}
