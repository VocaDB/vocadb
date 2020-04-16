using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceFile : VideoService {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly HashSet<string> mimeTypes = new HashSet<string>(new[] { "audio/mpeg" }, StringComparer.InvariantCultureIgnoreCase);

		public VideoServiceFile() 
			: base(PVService.File, null, new[] {
				new RegexLinkMatcher("{0}", @"(.+\.mp3$)"),
			}) {}

		public override string GetIdByUrl(VocaDbUrl url) {
			return url.Url;
		}

		public override VocaDbUrl GetThumbUrlById(string id) {
			return VocaDbUrl.Empty;
		}

		public override VocaDbUrl GetMaxSizeThumbUrlById(string id) {
			return VocaDbUrl.Empty;
		}

		public override VocaDbUrl GetUrlById(string id, PVExtendedMetadata _) {
			return VocaDbUrl.External(id);
		}

		private VideoTitleParseResult GetVideoTitle(string id) {

			string name = string.Empty;
			if (Uri.TryCreate(id, UriKind.Absolute, out var uri)) {
				name = HttpUtility.UrlDecode(uri.Segments.Last());
			}

			return VideoTitleParseResult.CreateSuccess(name, string.Empty, string.Empty, VocaDbUrl.Empty);

		}

		public override Task<VideoTitleParseResult> GetVideoTitleAsync(string id) {
			return Task.FromResult(GetVideoTitle(id));
		}

		public override bool IsAuthorized(IUserPermissionContext permissionContext) {
			return permissionContext != null && permissionContext.HasPermission(PermissionToken.AddRawFileMedia);
		}

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(VocaDbUrl url, bool getTitle) {
			
			url = url.EnsureScheme().ToAbsolute();

			try {
				url.Validate();
			} catch (UriFormatException x) {
				var msg = string.Format("{0} could not be parsed as a valid URL.", url);
				log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));							
			}

			var request = new HttpRequestMessage(HttpMethod.Head, url.Url);
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue("VocaDB", "1.0"));

			try {

				using (var client = new HttpClient()) {

					client.Timeout = TimeSpan.FromSeconds(10);

					using (var response = await client.SendAsync(request)) {
						response.EnsureSuccessStatusCode();

						var mime = response.Content.Headers.ContentType?.MediaType;

						if (!mimeTypes.Contains(mime)) {
							return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, string.Format("Unsupported content type: {0}", mime));
						}

					}

				}

			} catch (WebException x) {
				var msg = string.Format("Unable to load file URL {0}. The file might not be publicly accessible", url);
				log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));			
			}

			return VideoUrlParseResult.CreateOk(url, PVService.File, url.Url, await GetVideoTitleAsync(url.Url));

		}

		protected override Task<VideoUrlParseResult> ParseByIdAsync(string id, VocaDbUrl url, bool getMeta) {
			return ParseByUrlAsync(url, getMeta);
		}

	}

}
