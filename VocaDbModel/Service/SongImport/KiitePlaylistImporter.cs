using System.Text.RegularExpressions;
using NLog;
using VocaDb.Model.DataContracts.SongImport;

namespace VocaDb.Model.Service.SongImport;

public sealed partial class KiitePlaylistImporter : ISongListImporter
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	[GeneratedRegex(@"^https?://kiite\.jp/playlist/([\w]{10})$")]
	private static partial Regex GeneratedRegex();

	public bool MatchUrl(string url)
	{
		return GeneratedRegex().IsMatch(url);
	}

	private static string GetId(string url)
	{
		var match = GeneratedRegex().Match(url);

		if (!match.Success)
		{
			s_log.Warn($"Kiite URL not regonized: {url.Replace(Environment.NewLine, "")}");
			throw new UnableToImportException($"Kiite URL not regonized: {url}");
		}

		var id = match.Groups[1].Value;
		return id;
	}

	public Task<PartialImportedSongs> GetSongsAsync(string url, string nextPageToken, int maxResults, bool parseAll)
	{
		throw new NotImplementedException();
	}

	public Task<ImportedSongListContract> ParseAsync(string url, bool parseAll)
	{
		var id = GetId(url);

		throw new NotImplementedException();
	}
}
