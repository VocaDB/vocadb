using System;
using Google.YouTube;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

		private int? GetLength(Video video) {

			if (video.Media == null || video.Media.Duration == null || string.IsNullOrEmpty(video.Media.Duration.Seconds))
				return null;

			int sec;
			if (int.TryParse(video.Media.Duration.Seconds, out sec))
				return sec;
			else
				return null;

		}

		public VideoTitleParseResult GetTitle(string id) {

			var settings = new YouTubeRequestSettings("VocaDB", null);
			var request = new YouTubeRequest(settings);
			var videoEntryUrl = new Uri(string.Format("https://gdata.youtube.com/feeds/api/videos/{0}", id)); // Loading by HTTPS gives us HTTPS thumbnails as well

			try {

				var video = request.Retrieve<Video>(videoEntryUrl);
				var thumbUrl = video.Thumbnails.Count > 0 ? video.Thumbnails[0].Url : string.Empty;
				var length = GetLength(video);

				return VideoTitleParseResult.CreateSuccess(video.Title, video.Author, thumbUrl, length);

			} catch (Exception x) {
				return VideoTitleParseResult.CreateError(x.Message);
			}

		}

	}

}
