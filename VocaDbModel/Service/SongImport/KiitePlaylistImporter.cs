using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.SongImport;

file sealed record KiitePlaylistItem(
	[JsonProperty("song_title")]
	string SongTitle,
	[JsonProperty("video_id")]
	string VideoId,
	[JsonProperty("video_url")]
	string VideoUrl
);

file sealed record KiitePlaylistResponse(
	string Description,
	[JsonProperty("list_title")]
	string ListTitle,
	KiitePlaylistItem[] Songs
);

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

	private async static Task<ImportedSongListContract> ParseAsync(string url)
	{
		var id = GetId(url);
		var response = await JsonRequest.ReadObjectAsync<KiitePlaylistResponse>($"https://kiite.jp/api/playlist/{id}")
			?? throw new InvalidOperationException();

		return new ImportedSongListContract(
			name: response.ListTitle,
			createDate: DateTime.Now,
			description: response.Description,
			songs: new PartialImportedSongs(
				items: response.Songs
					.Select((song, index) => new ImportedSongInListContract
					{
						Name = song.SongTitle,
						PVId = song.VideoId,
						PVService = PVService.NicoNicoDouga,
						SortIndex = index + 1,
						Url = song.VideoUrl,
					})
					.ToArray(),
				totalCount: response.Songs.Length,
				nextPageToken: null
			),
			wvrId: 0
		);
	}

	public async Task<ImportedSongListContract> ParseAsync(string url, bool parseAll)
	{
		try
		{
			return await ParseAsync(url);
		}
		catch (Exception x)
		{
			s_log.Warn(x, "Unable to read Kiite playlist");
			throw new UnableToImportException("Unable to read Kiite playlist");
		}
	}
}
