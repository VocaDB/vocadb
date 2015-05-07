using System;
using System.Linq;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
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
				var thumbUrl = video.Snippet.Thumbnails.Default != null ? video.Snippet.Thumbnails.Default.Url : string.Empty;
				var length = GetLength(video);
				var author = video.Snippet.ChannelTitle;
			
				return VideoTitleParseResult.CreateSuccess(video.Snippet.Title, author, thumbUrl, length);

			} catch (Exception x) {
				return VideoTitleParseResult.CreateError(x.Message);
			}

		}

	}

}
