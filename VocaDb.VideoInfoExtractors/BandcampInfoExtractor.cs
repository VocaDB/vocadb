// Code from: https://github.com/blackjack4494/yt-dlc/blob/0704d2224b328caeafbce6a029904472628d12bd/youtube_dlc/extractor/bandcamp.py

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Html.Parser;
using Newtonsoft.Json.Linq;

namespace VocaDb.VideoInfoExtractors {

	public record BandcampInfo {

		/// <summary>
		/// Length of the video in seconds, as an integer or float.
		/// </summary>
		public double? Duration { get; init; }

		/// <summary>
		/// Video identifier.
		/// </summary>
		public string? Id { get; init; }

		/// <summary>
		/// Full URL to a video thumbnail image.
		/// </summary>
		public string? Thumbnail { get; init; }

		/// <summary>
		/// Video title, unescaped.
		/// </summary>
		public string? Title { get; init; }

		/// <summary>
		/// Video upload date (YYYYMMDD).
		/// If not explicitly set, calculated from timestamp.
		/// </summary>
		public string? UploadDate { get; init; }

		/// <summary>
		/// Full name of the video uploader.
		/// </summary>
		public string? Uploader { get; init; }

		/// <summary>
		/// Nickname or id of the video uploader.
		/// </summary>
		public string? UploaderId { get; init; }

		/// <summary>
		/// The URL to the video webpage, if given to youtube-dlc it
		/// should allow to get the same result again. (It will be set
		/// by YoutubeDL if it's missing)
		/// </summary>
		public string? WebpageUrl { get; init; }

	}

	public class BandcampInfoExtractor {

		private sealed record TrackInfo {

			public double? Duration { get; init; }

			public string? Id { get; init; }

			public string? Title { get; init; }

		}

		private JObject ExtractJsonFromHtmlDataAttribute(string webpage, string suffix) {
			var match = Regex.Match(webpage, $@" data-{suffix}=""([^""]*)");
			return JObject.Parse(HttpUtility.HtmlDecode(match.Groups[1].Value));
		}

		private TrackInfo ParseJsonTrack(JToken json) {
			return new TrackInfo {
				Duration = json["duration"]?.Value<double>(),
				Id = json["track_id"]?.Value<string>() ?? json["id"]?.Value<string>(),
				Title = json["title"]?.Value<string>(),
			};
		}

		public async Task<BandcampInfo> ExtractAsync(string url) {
			var match = Regex.Match(url, @"https?://[^/]+\.bandcamp\.com/track/([^/?#&]+)");
			var title = match.Groups[1].Value;
			// TODO: use VocaDb.Model.Service.Security.SslHelper.ForceStrongTLS
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			using var httpClient = new HttpClient();
			var webpage = await httpClient.GetStringAsync(url).ConfigureAwait(false);
			var parser = new HtmlParser();
			var document = await parser.ParseDocumentAsync(webpage).ConfigureAwait(false);
			var thumbnail = document.QuerySelector(@"meta[property~=""og:image""]").Attributes["content"].Value;

			var jsonTralbum = ExtractJsonFromHtmlDataAttribute(webpage, "tralbum");
			var jsonEmbed = ExtractJsonFromHtmlDataAttribute(webpage, "embed");

			var jsonTracks = jsonTralbum["trackinfo"] ?? throw new FormatException("Could not extract track");

			var track = ParseJsonTrack(jsonTracks.First());
			var artist = jsonTralbum["artist"]?.Value<string>();

			var albumPublishDate = jsonTralbum["packages"] is JToken jsonAlbum && jsonAlbum.Any() ? jsonAlbum.First()["album_publish_date"]?.Value<string>() : null;

			var timestamp = jsonTralbum["current"]?["publish_date"]?.Value<string>() ?? albumPublishDate;

			var downloadLink = Regex.Match(webpage, @"freeDownloadPage(?:[""\']|&quot;):\s*([""\']|&quot;)((?:(?!\1).)+)\1");
			if (downloadLink.Success) {
				// TODO: implement
			}

			title = artist != null ? $"{artist} - {track.Title}" : track.Title;

			return new BandcampInfo {
				Duration = track.Duration,
				Id = track.Id,
				Thumbnail = thumbnail,
				Title = title,
				UploadDate = DateTime.TryParse(timestamp, out var uploadDate) ? uploadDate.ToUniversalTime().ToString("yyyyMMdd") : null,
				Uploader = artist,
				WebpageUrl = url,
			};
		}

	}

}
