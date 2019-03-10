using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;
using NicoApi;
using NLog;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoParser : IVideoServiceParser {
		public Task<VideoTitleParseResult> GetTitleAsync(string id) => NicoHelper.GetTitleAPIAsync(id);
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

		/// <summary>
		/// Legacy method for retrieving user name using HTML parsing.
		/// This should be included in the API now.
		/// </summary>
		private static string GetUserName(string userId) {

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

		public static IEnumerable<string> GetUserProfileUrlById(string userId) {
			return new [] {
				string.Format("http://www.nicovideo.jp/user/{0}", userId),
				string.Format("https://www.nicovideo.jp/user/{0}", userId),
			};
		}

		public static NicoResponse GetResponse(Stream stream) {
			var serializer = new XmlSerializer(typeof(NicoResponse));
			return (NicoResponse)serializer.Deserialize(stream);
		}

		public static VideoTitleParseResult ParseResponse(VideoDataResult nicoResponse) {

			string author = nicoResponse.Author;
			string userId = nicoResponse.AuthorId;
			if (string.IsNullOrEmpty(author))
				author = GetUserName(userId);

			var result = VideoTitleParseResult.CreateSuccess(nicoResponse.Title, author, userId, nicoResponse.ThumbUrl, nicoResponse.LengthSeconds, uploadDate: nicoResponse.UploadDate?.Date);
			result.Tags = nicoResponse.Tags.Select(tag => tag.Name).ToArray();

			return result;

		}

		public static async Task<VideoTitleParseResult> GetTitleAPIAsync(string id) {

			VideoDataResult result;
			try {
				result = await new NicoApiClient(HtmlEntity.DeEntitize).GetTitleAPIAsync(id);
			} catch (NicoApiException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
			}
			return ParseResponse(result);

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

}
