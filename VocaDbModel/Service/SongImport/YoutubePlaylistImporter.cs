#nullable disable

using System.Text.RegularExpressions;
using NLog;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices.Youtube;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.SongImport;

public class YoutubePlaylistImporter : ISongListImporter
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	private static readonly Regex s_regex = new(@"www\.youtube\.com/playlist\?list=([\w\-_]+)");

	private const string PlaylistsFormat =
		"https://www.googleapis.com/youtube/v3/playlists?part=snippet&key={0}&id={1}";

	private const string PlaylistItemsFormat =
		"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&key={0}&playlistId={1}&maxResults={2}&pageToken={3}";

	private string YoutubeApiKey => AppConfig.YoutubeApiKey;

	private string GetId(string url)
	{
		var match = s_regex.Match(url);

		if (!match.Success)
		{
			s_log.Warn($"Youtube URL not regonized: {url.Replace(Environment.NewLine, "")}");
			throw new UnableToImportException($"Youtube URL not regonized: {url}");
		}

		var id = match.Groups[1].Value;
		return id;
	}

	private async Task<PartialImportedSongs> GetSongsById(string playlistId, string pageToken, int maxResults, bool parseAll)
	{
		var songs = new List<ImportedSongInListContract>();

		var requestUrl = string.Format(PlaylistItemsFormat, YoutubeApiKey, playlistId, maxResults, pageToken);

		YoutubePlaylistItemResponse result;

		try
		{
			result = await JsonRequest.ReadObjectAsync<YoutubePlaylistItemResponse>(requestUrl);
		}
		catch (Exception x)
		{
			s_log.Warn(x, "Unable to read Youtube playlist");
			throw new UnableToImportException("Unable to read Youtube playlist", x);
		}

		foreach (var item in result.Items)
		{
			var song = new ImportedSongInListContract(PVService.Youtube, item.Snippet.ResourceId.VideoId)
			{
				Name = item.Snippet.Title,
				SortIndex = ((int?)item.Snippet.Position ?? 0) + 1,
				Url = $"https://www.youtube.com/watch?v={item.Snippet.ResourceId.VideoId}"
			};
			songs.Add(song);
		}

		return new PartialImportedSongs(songs.ToArray(), result.PageInfo.TotalResults ?? 0, result.NextPageToken);
	}

	public Task<PartialImportedSongs> GetSongsAsync(string url, string nextPageToken, int maxResults, bool parseAll)
	{
		return GetSongsById(GetId(url), nextPageToken, maxResults, parseAll);
	}

	public async Task<ImportedSongListContract> ParseAsync(string url, bool parseAll)
	{
		var id = GetId(url);

		var requestUrl = string.Format(PlaylistsFormat, YoutubeApiKey, id);
		YoutubePlaylistResponse result;

		try
		{
			result = await JsonRequest.ReadObjectAsync<YoutubePlaylistResponse>(requestUrl);
		}
		catch (Exception x)
		{
			s_log.Warn(x, "Unable to read Youtube playlist");
			throw new UnableToImportException("Unable to read Youtube playlist");
		}

		if (!result.Items.Any())
		{
			s_log.Info("Youtube playlist not found");
			throw new UnableToImportException($"Youtube playlist not found: {url}");
		}

		var name = result.Items[0].Snippet.Title;
		var description = result.Items[0].Snippet.Description;
		var created = (result.Items[0].Snippet.PublishedAt ?? DateTimeOffset.Now).DateTime;

		var songs = await GetSongsById(id, null, 10, parseAll);

		return new ImportedSongListContract { Name = name, Description = description, CreateDate = created, Songs = songs };
	}

	public bool MatchUrl(string url)
	{
		return s_regex.IsMatch(url);
	}

	public class YoutubePlaylistResponse : YoutubeResponse<YoutubePlaylistItem> { }

	public class YoutubePlaylistItem : YoutubeItem<Snippet> { }

	public class YoutubePlaylistItemResponse : YoutubeResponse<YoutubePlaylistItemItem> { }

	public class YoutubePlaylistItemItem : YoutubeItem<YoutubePlaylistItemSnippet> { }

	public class YoutubePlaylistItemSnippet : Snippet
	{
		public uint? Position { get; set; }

		public YoutubeResourceId ResourceId { get; set; }
	}

	public class YoutubeResourceId
	{
		public string VideoId { get; set; }
	}
}
