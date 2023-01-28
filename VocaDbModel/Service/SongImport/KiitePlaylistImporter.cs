using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;

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

	private sealed record KiitePlaylistItem(
		[JsonProperty("song_title")]
		string SongTitle,
		[JsonProperty("video_id")]
		string VideoId,
		[JsonProperty("video_url")]
		string VideoUrl
	);

	private sealed record KiitePlaylistResponse(
		string Description,
		[JsonProperty("list_title")]
		string ListTitle,
		KiitePlaylistItem[] Songs
	);

	private static IEnumerable<KiitePlaylistItem> ParsePlaylistItems(IHtmlDocument document)
	{
		static KiitePlaylistItem CreateKiitePlaylistItemFromLiElement(IElement element)
		{
			var songTitle = element.Attributes["data-song-title"]?.Value ?? throw new FormatException("Unable to extract song title");
			var videoId = element.Attributes["data-video-id"]?.Value ?? throw new FormatException("Unable to extract video id");

			return new KiitePlaylistItem(
				SongTitle: songTitle,
				VideoId: videoId,
				VideoUrl: $"https://www.nicovideo.jp/watch/{videoId}"
			);
		}

		return document.QuerySelectorAll("ol.playlist.col-playlist > li").Select(CreateKiitePlaylistItemFromLiElement);
	}

	private static KiitePlaylistResponse ParseHtml(IHtmlDocument document)
	{
		var contentDescription = document.QuerySelector("#content-description")?.TextContent ?? throw new FormatException("Unable to extract content description");
		var description = !string.IsNullOrEmpty(contentDescription)
			? document.QuerySelector(@"meta[name~=""description""]")?.Attributes["content"]?.Value ?? throw new FormatException("Unable to extract description")
			: string.Empty;
		var listTitle = document.QuerySelector("#playlist-info")?.Attributes["data-playlist-title"]?.Value ?? throw new FormatException("Unable to extract playlist title");
		var items = ParsePlaylistItems(document);

		return new KiitePlaylistResponse(
			Description: description,
			ListTitle: listTitle,
			Songs: items.ToArray()
		);
	}

	private async static Task<ImportedSongListContract> ParseAsync(string url)
	{
		var id = GetId(url);
		var document = await HtmlRequestHelper.GetStreamAsync($"https://kiite.jp/playlist/{id}", stream =>
		{
			var parser = new HtmlParser();
			var document = parser.ParseDocument(stream);
			return document;
		});
		var response = ParseHtml(document);

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
