using System;
using System.Linq;
using System.Xml;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

		private const string videoQueryFormat =
			"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&key={0}&id={1}";

		private int? GetLength(YoutubeVideoItem video) {

			if (video.ContentDetails == null || string.IsNullOrEmpty(video.ContentDetails.Duration))
				return null;

			TimeSpan timespan;

			try {
				timespan = XmlConvert.ToTimeSpan(video.ContentDetails.Duration);			
			} catch (FormatException) {
				return null;
			}

			return (int?)timespan.TotalSeconds;

		}

		private DateTime? GetPublishDate(YoutubeVideoItem video) {
			return video.Snippet.PublishedAt.Date;
		}

		public VideoTitleParseResult GetTitle(string id) {

			var apiKey = AppConfig.YoutubeApiKey;

			var url = string.Format(videoQueryFormat, apiKey, id);

			YoutubeResponse result;
			try {
				result = JsonRequest.ReadObject<YoutubeResponse>(url);
			} catch (Exception x) {
				return VideoTitleParseResult.CreateError(x.Message);
			}

			if (!result.Items.Any()) {
				return VideoTitleParseResult.Empty;
			}

			var video = result.Items.First();
			var thumbUrl = video.Snippet.Thumbnails.Default != null ? video.Snippet.Thumbnails.Default.Url : string.Empty;
			var length = GetLength(video);
			var author = video.Snippet.ChannelTitle;
			var publishDate = GetPublishDate(video);

			return VideoTitleParseResult.CreateSuccess(video.Snippet.Title, author, thumbUrl, length, uploadDate: publishDate);

		}

		public class YoutubeResponse {
			
			public YoutubeVideoItem[] Items { get; set; }

		}

		public class YoutubeVideoItem {
			
			public YoutubeVideoSnippet Snippet { get; set; }

			public YoutubeVideoContentDetails ContentDetails { get; set; }

		}

		public class YoutubeVideoSnippet {
			
			public string ChannelTitle { get; set; }

			public DateTimeOffset PublishedAt { get; set; }

			public YoutubeVideoThumbnails Thumbnails { get; set; }

			public string Title { get; set; }

		}

		public class YoutubeVideoThumbnails {
			
			public YoutubeVideoThumbnail Default { get; set; }

		}

		public class YoutubeVideoThumbnail {
			
			public string Url { get; set;  }

		}

		public class YoutubeVideoContentDetails {
			
			public string Duration { get; set; }

		}

	}

}
