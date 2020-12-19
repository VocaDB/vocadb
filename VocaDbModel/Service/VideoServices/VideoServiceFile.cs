#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using NLog;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.VideoServices
{
	public class VideoServiceFile : VideoService
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private static readonly HashSet<string> s_mimeTypes = new(new[] { "audio/mpeg" }, StringComparer.InvariantCultureIgnoreCase);

		public VideoServiceFile()
			: base(PVService.File, null, new[] {
				new RegexLinkMatcher("{0}", @"(.+\.mp3$)"),
			})
		{ }

		public override string GetIdByUrl(string url)
		{
			return url;
		}

		public override string GetThumbUrlById(string id)
		{
			return string.Empty;
		}

		public override string GetMaxSizeThumbUrlById(string id)
		{
			return string.Empty;
		}

		public override string GetUrlById(string id, PVExtendedMetadata _)
		{
			return id;
		}

		private VideoTitleParseResult GetVideoTitle(string id)
		{
			string name = string.Empty;
			if (Uri.TryCreate(id, UriKind.Absolute, out Uri uri))
				name = HttpUtility.UrlDecode(uri.Segments.Last());

			return VideoTitleParseResult.CreateSuccess(name, string.Empty, string.Empty, string.Empty);
		}

		public override Task<VideoTitleParseResult> GetVideoTitleAsync(string id)
		{
			return Task.FromResult(GetVideoTitle(id));
		}

		public override bool IsAuthorized(IUserPermissionContext permissionContext)
		{
			return permissionContext != null && permissionContext.HasPermission(PermissionToken.AddRawFileMedia);
		}

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle)
		{
			url = UrlHelper.MakeLink(url);

			Uri parsedUri;
			try
			{
				parsedUri = new Uri(url, UriKind.Absolute);
			}
			catch (UriFormatException x)
			{
				var msg = $"{url} could not be parsed as a valid URL.";
				s_log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));
			}

			var request = new HttpRequestMessage(HttpMethod.Head, url);
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue("VocaDB", "1.0"));

			try
			{
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(10);

					using (var response = await client.SendAsync(request))
					{
						response.EnsureSuccessStatusCode();

						var mime = response.Content.Headers.ContentType?.MediaType;

						if (!s_mimeTypes.Contains(mime))
						{
							return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, $"Unsupported content type: {mime}");
						}
					}
				}
			}
			catch (WebException x)
			{
				var msg = $"Unable to load file URL {url}. The file might not be publicly accessible";
				s_log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));
			}

			return VideoUrlParseResult.CreateOk(url, PVService.File, url, await GetVideoTitleAsync(url));
		}

		protected override Task<VideoUrlParseResult> ParseByIdAsync(string id, string url, bool getMeta)
		{
			return ParseByUrlAsync(url, getMeta);
		}
	}
}
