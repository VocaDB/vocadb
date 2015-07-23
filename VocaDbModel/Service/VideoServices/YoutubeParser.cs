using System;
using System.Linq;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

		private int? GetLength(Video video) {

			if (video.ContentDetails == null || string.IsNullOrEmpty(video.ContentDetails.Duration))
				return null;

			TimeSpan timespan;

			try {
				timespan = System.Xml.XmlConvert.ToTimeSpan(video.ContentDetails.Duration);			
			} catch (FormatException) {
				return null;
			}

			return (int?)timespan.TotalSeconds;

		}

		private DateTime? GetPublishDate(Video video) {
			return DateTimeHelper.ParseDateTimeOffsetAsDate(video.Snippet.PublishedAtRaw);
		}

		public VideoTitleParseResult GetTitle(string id) {

			var apiKey = AppConfig.YoutubeApiKey;

			var youtubeService = new YouTubeService(new BaseClientService.Initializer {
				ApiKey = apiKey,
				ApplicationName = "VocaDB"
			});			

			var request = youtubeService.Videos.List("snippet,contentDetails");
			request.Id = id;

			try {

				var result = request.Execute();
				
				if (!result.Items.Any()) {
					return VideoTitleParseResult.Empty;
				}

				var video = result.Items.First();
				var thumbUrl = video.Snippet.Thumbnails.Default__ != null ? video.Snippet.Thumbnails.Default__.Url : string.Empty;
				var length = GetLength(video);
				var author = video.Snippet.ChannelTitle;
				var publishDate = GetPublishDate(video);
			
				return VideoTitleParseResult.CreateSuccess(video.Snippet.Title, author, thumbUrl, length, uploadDate: publishDate);

			} catch (Exception x) {
				return VideoTitleParseResult.CreateError(x.Message);
			}

		}

	}

}
