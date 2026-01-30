// Code from: https://github.com/blackjack4494/yt-dlc/blob/0704d2224b328caeafbce6a029904472628d12bd/youtube_dlc/extractor/bandcamp.py

using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json.Linq;

namespace VocaDb.BandcampMetadataExtractor
{
	public class BandcampMetadataClient
	{
		private sealed record TrackInfo
		{
			public double? Duration { get; init; }

			public string? Id { get; init; }

			public string? Title { get; init; }
		}

		private JObject ExtractJsonFromHtmlDataAttribute(IHtmlDocument document, string suffix)
		{
			var name = $"data-{suffix}";
			var match = document.QuerySelector($"[{name}]");
			return JObject.Parse(HttpUtility.HtmlDecode(match.Attributes[name].Value));
		}

		private TrackInfo ParseJsonTrack(JToken json)
		{
			return new TrackInfo
			{
				Duration = json["duration"]?.Value<double>(),
				Id = json["track_id"]?.Value<string>() ?? json["id"]?.Value<string>(),
				Title = json["title"]?.Value<string>(),
			};
		}

		public async Task<BandcampInfo> ExtractAsync(string url)
		{
			//var match = Regex.Match(url, @"https?://[^/]+\.bandcamp\.com/track/([^/?#&]+)");
			//var title = match.Groups[1].Value;
			SslHelper.ForceStrongTLS();
			using var httpClient = new HttpClient();
			var webpage = await httpClient.GetStringAsync(url).ConfigureAwait(false);
			var parser = new HtmlParser();
			var document = await parser.ParseDocumentAsync(webpage).ConfigureAwait(false);
			var thumbnail = document.QuerySelector(@"meta[property~=""og:image""]").Attributes["content"].Value;

			var jsonTralbum = ExtractJsonFromHtmlDataAttribute(document, "tralbum");
			var jsonEmbed = ExtractJsonFromHtmlDataAttribute(document, "embed");

			var jsonTracks = jsonTralbum["trackinfo"] ?? throw new FormatException("Could not extract track");

			var track = ParseJsonTrack(jsonTracks.First());
			var artist = jsonTralbum["artist"]?.Value<string>();

			var albumPublishDate = jsonTralbum["packages"] is JToken jsonAlbum && jsonAlbum.Any() ? jsonAlbum.First()["album_publish_date"]?.Value<string>() : null;

			var timestamp = jsonTralbum["current"]?["publish_date"]?.Value<string>() ?? albumPublishDate;

			// Extract description from current.about or og:description meta tag
			var description = jsonTralbum["current"]?["about"]?.Value<string>();
			if (string.IsNullOrEmpty(description))
			{
				description = document.QuerySelector(@"meta[property~=""og:description""]")?.Attributes["content"]?.Value;
			}

			var downloadLink = Regex.Match(webpage, @"freeDownloadPage(?:[""\']|&quot;):\s*([""\']|&quot;)((?:(?!\1).)+)\1");
			if (downloadLink.Success)
			{
				// TODO: implement
			}

			var title = artist != null ? $"{artist} - {track.Title}" : track.Title;

			return new BandcampInfo
			{
				Description = description,
				Duration = track.Duration,
				Id = track.Id,
				Thumbnail = thumbnail,
				Title = title,
				UploadDate = DateTimeOffset.TryParse(timestamp, out var uploadDate) ? uploadDate.ToUniversalTime().ToString("yyyyMMdd") : null,
				Uploader = artist,
				WebpageUrl = url,
			};
		}
	}
}
