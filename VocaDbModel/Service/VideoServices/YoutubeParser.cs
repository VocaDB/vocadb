using System;
using System.Linq;
using System.Net;
using System.Xml;
using VocaDb.Model.Service.VideoServices.Youtube;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

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
			return (video.Snippet.PublishedAt.HasValue ? (DateTime?)video.Snippet.PublishedAt.Value.Date : null);
		}

		public VideoTitleParseResult GetTitle(string id) {

			var service = new YoutubeService(AppConfig.YoutubeApiKey);

			YoutubeVideoResponse result;
			try {
				result = service.Video(id);
            } catch (WebException x) {
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

			var res = VideoTitleParseResult.CreateSuccess(video.Snippet.Title, author, thumbUrl, length, uploadDate: publishDate);
			res.AuthorId = video.Snippet.ChannelId;
			return res;

		}

	}

}
