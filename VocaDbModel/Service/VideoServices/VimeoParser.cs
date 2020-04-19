using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.VideoServices {

	public class VimeoParser : IVideoServiceParser {

		private VideoTitleParseResult GetTitle(string id) {

			var url = string.Format("http://vimeo.com/api/v2/video/{0}.xml", id);

			var request = WebRequest.Create(url);
			var serializer = new XmlSerializer(typeof(VimeoResult));
			VimeoResult result;

			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {
					result = (VimeoResult)serializer.Deserialize(stream);					
				}
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}

			if (result == null || result.Video == null || string.IsNullOrEmpty(result.Video.Title)) {
				return VideoTitleParseResult.CreateError("Vimeo (error): title element not found");
			}

			var author = result.Video.User_Name;
			var thumbUrl = VocaDbUrl.External(result.Video.Thumbnail_Small);
			var length = result.Video.Duration;
			var date = Convert.ToDateTime(result.Video.Upload_Date); // xmlserializer can't parse the date

			return VideoTitleParseResult.CreateSuccess(result.Video.Title, author, null, thumbUrl, length, uploadDate: date);

		}

		public Task<VideoTitleParseResult> GetTitleAsync(string id) => Task.FromResult(GetTitle(id));

	}

	[XmlRoot("videos", Namespace = "")]
	public class VimeoResult {
		
		[XmlElement("video")]
		public VimeoVideo Video { get; set; }

	}

	public class VimeoVideo {
		
		[XmlElement("duration")]
		public int Duration { get; set; }

		[XmlElement("thumbnail_small")]
		public string Thumbnail_Small { get; set; }

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("upload_date")]
		public string Upload_Date { get; set; }

		[XmlElement("user_name")]
		public string User_Name { get; set; }

	}

}
