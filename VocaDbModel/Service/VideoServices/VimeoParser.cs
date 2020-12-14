#nullable disable

using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices
{
	public class VimeoParser : IVideoServiceParser
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public async Task<VideoTitleParseResult> GetTitleAsync(string id)
		{
			static void SetHeaders(HttpRequestHeaders headers)
			{
				var apiKey = AppConfig.VimeoApiKey;
				headers.Authorization = new AuthenticationHeaderValue("bearer", apiKey);
			}

			var url = string.Format("https://api.vimeo.com/videos/{0}", id);

			VimeoResult result;

			SslHelper.ForceStrongTLS();

			try
			{
				result = await JsonRequest.ReadObjectAsync<VimeoResult>(url, TimeSpan.FromSeconds(100), headers: SetHeaders);
			}
			catch (WebException x)
			{
				log.Warn(x, "Unable to load Vimeo URL {0}", url);
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}
			catch (HttpRequestException x)
			{
				log.Warn(x, "Unable to load Vimeo URL {0}", url);
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}
			catch (JsonSerializationException x)
			{
				log.Warn(x, "Unable to load Vimeo URL {0}", url);
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}

			var title = result.Name;

			if (string.IsNullOrEmpty(title))
			{
				return VideoTitleParseResult.CreateError("Vimeo (error): title element not found");
			}

			var author = result.User?.Name ?? "";
			var thumbUrl = result.Pictures.Sizes.Any() ? result.Pictures.Sizes.OrderBy(p => p.Width).First().Link : "";
			var length = result.Duration;
			var date = result.CreatedTime; // Convert.ToDateTime(result.Video.Upload_Date); // xmlserializer can't parse the date

			return VideoTitleParseResult.CreateSuccess(title, author, null, thumbUrl, length, uploadDate: date);
		}
	}

	public class VimeoResult
	{
		public int Duration { get; set; }

		public string Name { get; set; }

		[JsonProperty("created_time")]
		public DateTime CreatedTime { get; set; }

		public VimeoPictures Pictures { get; set; }

		public VimeoUser User { get; set; }
	}

	public class VimeoPictures
	{
		public VimeoPicture[] Sizes { get; set; }
	}

	public class VimeoPicture
	{
		public string Link { get; set; }

		public int Width { get; set; }
	}

	public class VimeoUser
	{
		public string Name { get; set; }
	}
}
