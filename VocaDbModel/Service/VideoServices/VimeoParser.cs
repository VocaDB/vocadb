using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class VimeoParser : IVideoServiceParser {

		private int? GetLength(string lengthStr) {

			int val;
			if (int.TryParse(lengthStr, out val))
				return val;
			else
				return null;

		}

		public VideoTitleParseResult GetTitle(string id) {

			var url = string.Format("http://vimeo.com/api/v2/video/{0}.xml", id);

			var request = WebRequest.Create(url);
			XDocument doc;

			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {
					doc = XDocument.Load(stream);					
				}
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			} catch (XmlException x) {
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}

			var titleElem = doc.XPathSelectElement("videos/video/title");

			if (titleElem == null) {
				return VideoTitleParseResult.CreateError("Vimeo (error): title element not found");
			}

			var author = XmlHelper.GetNodeTextOrEmpty(doc, "videos/video/user_name");
			var thumbUrl = XmlHelper.GetNodeTextOrEmpty(doc, "videos/video/thumbnail_small");
			var length = GetLength(XmlHelper.GetNodeTextOrEmpty(doc, "videos/video/duration"));

			return VideoTitleParseResult.CreateSuccess(titleElem.Value, author, thumbUrl, length);

		}

	}

}
