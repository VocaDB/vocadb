using System.Text.RegularExpressions;
using VocaDb.Model.DataContracts.SongImport;

namespace VocaDb.Model.Service.SongImport;

public sealed partial class KiitePlaylistImporter : ISongListImporter
{
	[GeneratedRegex(@"kiite\.jp/playlist/([\w]{10})")]
	private static partial Regex GeneratedRegex();

	public bool MatchUrl(string url)
	{
		return GeneratedRegex().IsMatch(url);
	}

	public Task<PartialImportedSongs> GetSongsAsync(string url, string nextPageToken, int maxResults, bool parseAll)
	{
		throw new NotImplementedException();
	}

	public Task<ImportedSongListContract> ParseAsync(string url, bool parseAll)
	{
		throw new NotImplementedException();
	}
}
