using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceSoundCloud : VideoService {

		class SoundCloudResult {

			public string Artwork_url { get; set; }

			public DateTime Created_at { get; set; }

			public int Duration { get; set; }

			public string Id { get; set; }

			public string Title { get; set; }

			public SoundCloudUser User { get; set; }

		}

		class SoundCloudUser {

			public string Avatar_url { get; set; }

			public string Permalink { get; set; }

			public string Username { get; set; }

		}

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public VideoServiceSoundCloud(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override string GetUrlById(string id, PVExtendedMetadata extendedMetadata = null) {

			var compositeId = new SoundCloudId(id);
			var matcher = linkMatchers.First();
			return $"http://{matcher.MakeLinkFromId(compositeId.SoundCloudUrl)}";

		}

		public async Task<VideoUrlParseResult> ParseBySoundCloudUrl(string url) {

			var apikey = AppConfig.SoundCloudClientId;
			var apiUrl = string.Format("https://api.soundcloud.com/resolve?url=http://soundcloud.com/{0}&client_id={1}", url, apikey);

			SoundCloudResult result;

			bool HasStatusCode(WebException x, HttpStatusCode statusCode) => x.Response != null && ((HttpWebResponse)x.Response).StatusCode == statusCode;
			
			VideoUrlParseResult ReturnError(Exception x, string additionalInfo = null) {
				var msg = string.Format("Unable to load SoundCloud URL '{0}'.{1}", url, additionalInfo != null ? " " + additionalInfo + ".": string.Empty);
				log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));
			}

			try {
				result = await JsonRequest.ReadObjectAsync<SoundCloudResult>(apiUrl, timeoutMs: 10000);
			} catch (WebException x) when (HasStatusCode(x, HttpStatusCode.Forbidden)) {
				// Forbidden most likely means the artist has prevented API access to their tracks, http://stackoverflow.com/a/36529330
				return ReturnError(x, "This track cannot be embedded");
			} catch (WebException x) when (HasStatusCode(x, HttpStatusCode.NotFound)) {
				return ReturnError(x, "Not found");
			} catch (WebException x) {
				return ReturnError(x);
			} catch (JsonSerializationException x) {
				return ReturnError(x);
			}

			var trackId = result.Id;
			var title = result.Title;

			if (trackId == null || title == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Unable to load SoundCloud URL: Invalid response.");

			var author = result.User.Username;
			var length = result.Duration / 1000;

			var thumbUrl = result.Artwork_url;

			// Substitute song thumbnail with user avatar, if no actual thumbnail is provided. This is what the SoundCloud site does as well.
			if (string.IsNullOrEmpty(thumbUrl)) {
				thumbUrl = result.User.Avatar_url;
			}

			var uploadDate = result.Created_at;

			var id = new SoundCloudId(trackId, url);
			var authorId = result.User.Permalink; // Using permalink because that's the public URL

			return VideoUrlParseResult.CreateOk(url, PVService.SoundCloud, id.ToString(), VideoTitleParseResult.CreateSuccess(title, author, authorId, thumbUrl, length, uploadDate: uploadDate));

		}

		public override Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			var soundCloudUrl = linkMatchers[0].GetId(url);

			return ParseBySoundCloudUrl(soundCloudUrl);

		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId) {
			return new[] { 
				string.Format("http://soundcloud.com/{0}", authorId),
				string.Format("https://soundcloud.com/{0}", authorId),
			};
		}

	}

	/// <summary>
	/// Composite SoundCloud ID. Contains both the track Id and the relative URL (for direct links).
	/// </summary>
	public class SoundCloudId {

		/// <summary>
		/// Remove query string.
		/// See https://github.com/VocaDB/vocadb/issues/459
		/// </summary>
		private string CleanUrl(string url) => url.Split('?')[0];

		public SoundCloudId(string trackId, string soundCloudUrl) {

			ParamIs.NotNullOrEmpty(() => trackId);
			ParamIs.NotNullOrEmpty(() => soundCloudUrl);

			TrackId = trackId;
			SoundCloudUrl = CleanUrl(soundCloudUrl);

		}

		public SoundCloudId(string compositeId) {

			ParamIs.NotNull(() => compositeId);

			var parts = compositeId.Split(' ');

			if (parts.Length < 2) {
				throw new ArgumentException("Composite ID must contain both track Id and URL");
			}

			TrackId = parts[0];
			SoundCloudUrl = parts[1];

		}

		/// <summary>
		/// Relative URL, for example tamagotaso/nightcruise
		/// </summary>
		public string SoundCloudUrl { get; set; }

		/// <summary>
		/// Track ID, for example 8431571
		/// </summary>
		public string TrackId { get; set; }

		/// <summary>
		/// Gets the composite ID string with both the relative URL and track Id.
		/// </summary>
		/// <returns>Composite ID, for example "8431571 tamagotaso/nightcruise"</returns>
		public override string  ToString() {
			return string.Format("{0} {1}", TrackId, SoundCloudUrl);
		}

	}

}
