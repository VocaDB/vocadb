using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			return NicoHelper.GetTitleAPI(id);

		}

	}

	public static class NicoHelper {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static string GetUserName(Stream htmlStream, Encoding encoding) {

			var doc = new HtmlDocument();
			try {
				doc.Load(htmlStream, encoding);
			} catch (IOException x) {
				log.Warn(x, "Unable to load document for user name");
			}

			var titleElem = doc.DocumentNode.SelectSingleNode("//html/body/div/p[2]/a/strong");

			var titleText = (titleElem != null ? titleElem.InnerText : null);

			return (titleText != null ? HtmlEntity.DeEntitize(titleText) : null);

		}

		public static string GetUserName(string userId) {

			var url = string.Format("http://ext.nicovideo.jp/thumb_user/{0}", userId);

			var request = WebRequest.Create(url);
			request.Timeout = 10000;
			WebResponse response;

			try {
				response = request.GetResponse();	// Closed below
			} catch (WebException x) {
				log.Warn(x, "Unable to get response for user name");
				return null;
			}

			var enc = GetEncoding(response.Headers[HttpResponseHeader.ContentEncoding]);

			try {
				using (var stream = response.GetResponseStream()) {
					return GetUserName(stream, enc);
				}
			} finally {
				response.Close();
			}

		}

		public static string GetUserProfileUrlById(string userId) {
			return string.Format("http://www.nicovideo.jp/user/{0}", userId);
		}

		public static NicoResponse GetResponse(Stream stream) {
			var serializer = new XmlSerializer(typeof(NicoResponse));
			return (NicoResponse)serializer.Deserialize(stream);
		}

		public static VideoTitleParseResult ParseResponse(NicoResponse nicoResponse) {
			
			if (nicoResponse.Status == "fail") {
				var err = (nicoResponse.Error != null ? nicoResponse.Error.Description : "empty response");
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + err);
			}

			var title = HtmlEntity.DeEntitize(nicoResponse.Thumb.Title);
			var thumbUrl = UrlHelper.UpgradeToHttps(nicoResponse.Thumb.Thumbnail_Url) ?? string.Empty;
			var userId = nicoResponse.Thumb.User_Id ?? string.Empty;
			var length = ParseLength(nicoResponse.Thumb.Length);
			var author = nicoResponse.Thumb.User_Nickname;
			var publishDate = DateTimeHelper.ParseDateTimeOffsetAsDate(nicoResponse.Thumb.First_Retrieve);

			if (string.IsNullOrEmpty(author))
				author = GetUserName(userId);

			var result = VideoTitleParseResult.CreateSuccess(title, author, userId, thumbUrl, length, uploadDate: publishDate);
			result.Tags = nicoResponse.Thumb.Tags;

			return result;

		}

		public static VideoTitleParseResult GetTitleAPI(string id) {

			var url = string.Format("https://ext.nicovideo.jp/api/getthumbinfo/{0}", id);

			var request = WebRequest.Create(url);
			request.Timeout = 10000;

			NicoResponse nicoResponse;

			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {
					nicoResponse = GetResponse(stream);
				}
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
			} catch (XmlException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
			} catch (IOException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);				
			}

			return ParseResponse(nicoResponse);

		}

		private static Encoding GetEncoding(string encodingStr) {

			if (string.IsNullOrEmpty(encodingStr))
				return Encoding.UTF8;

			try {
				return Encoding.GetEncoding(encodingStr);
			} catch (ArgumentException) {
				return Encoding.UTF8;
			}

		}

		public static int? ParseLength(string lengthStr) {

			if (string.IsNullOrEmpty(lengthStr))
				return null;

			var parts = lengthStr.Split(':');

			if (parts.Length != 2)
				return null;

			int min, sec;
			if (!int.TryParse(parts[0], out min) || !int.TryParse(parts[1], out sec))
				return null;

			var totalSec = min*60 + sec;

			return totalSec;

		}

		/// <summary>
		/// Parses song title, artists and song type from NicoNico video title.
		/// </summary>
		/// <param name="title">NicoNico video title. Can be null or empty, in which case that value is returned.</param>
		/// <param name="artistFunc">Function for finding artist by name. Cannot be null.</param>
		/// <returns>Parse result. Cannot be null.</returns>
		/// <remarks>This works with titles that follow the common Nico format, such as 【重音テト】 ハイゲインワンダーランド 【オリジナル】.</remarks>
		public static NicoTitleParseResult ParseTitle(string title, Func<string, Artist> artistFunc) {

			if (string.IsNullOrEmpty(title))
				return new NicoTitleParseResult(title);

			var elemRegex = new Regex(@"【\s?([\w･・]+)\s?】");
			var matches = elemRegex.Matches(title);
			var artists = new List<Artist>();
			var songType = SongType.Unspecified;
			int offset = 0;

			if (matches.Count == 0)
				return new NicoTitleParseResult(title);

			foreach (Match match in matches) {

				var original = "オリジナル";
				var cover = "カバー";

				var content = match.Groups[1].Value.Trim();
				if (content == original)
					songType = SongType.Original;
				else if (content == cover)
					songType = SongType.Cover;
				else {

					if (content.Contains(original)) {
						songType = SongType.Original;
						content = content.Replace(original, string.Empty);
					} else if (content.Contains(cover)) {
						songType = SongType.Cover;
					}

					var a = artistFunc(content);
					if (a != null && !artists.Contains(a))
						artists.Add(a);
					else {
						
						var parts = content.Split('･', '・', '×');

						foreach (var part in parts.Where(p => p != string.Empty)) {
							
							a = artistFunc(part);

							// Some UTAUs are credited with the "音源" suffix, so also try without it.
							if (a == null && part.EndsWith("音源")) {
								a = artistFunc(part.Substring(0, part.Length - 2));
							}

							if (a != null)
								artists.Add(a);

						}

					}

				}

				title = title.Remove(match.Index - offset, match.Value.Length);
				offset += match.Length;

			}

			return new NicoTitleParseResult(title.Trim(), artists, songType);

		}

	}

	[XmlRoot(ElementName = "nicovideo_thumb_response", Namespace = "")]
	public class NicoResponse {
		
		[XmlElement("error")]
		public NicoResponseError Error { get; set; }

		[XmlAttribute("status")]
		public string Status { get; set; }

		[XmlElement("thumb")]
		public NicoResponseThumb Thumb { get; set; }

	}

	public class NicoResponseError {
		
		[XmlElement("code")]
		public string Code { get; set; }

		[XmlElement("description")]
		public string Description { get; set; }

	}

	public class NicoResponseThumb {
		
		[XmlElement("first_retrieve")]
		public string First_Retrieve { get; set; }

		[XmlElement("length")]
		public string Length { get; set; }

		[XmlArray("tags")]
		[XmlArrayItem("tag")]
		public string[] Tags { get; set; }

		[XmlElement("thumbnail_url")]
		public string Thumbnail_Url { get; set; }

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("user_id")]
		public string User_Id { get; set; }

		[XmlElement("user_nickname")]
		public string User_Nickname { get; set; }

		[XmlElement("video_id")]
		public string Video_Id { get; set; }

	}

}
