using System;
using System.IO;
using System.Linq;
using System.Net;
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

			public string Username { get; set; }

		}

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public VideoServiceSoundCloud(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override string GetUrlById(string id) {

			var compositeId = new SoundCloudId(id);
			var matcher = linkMatchers.First();
			return "http://" + matcher.MakeLinkFromId(compositeId.SoundCloudUrl);

		}

		public VideoUrlParseResult ParseBySoundCloudUrl(string url) {

			var apikey = AppConfig.SoundCloudClientId;
			var apiUrl = string.Format("http://api.soundcloud.com/resolve?url=http://soundcloud.com/{0}&client_id={1}", url, apikey);

			SoundCloudResult result;

			try {				
				result = JsonRequest.ReadObject<SoundCloudResult>(apiUrl, timeoutMs: 10000);
			} catch (WebException x) {

				var msg = string.Format("Unable to load SoundCloud URL '{0}'.", url);

				// Forbidden most likely means the artist has prevented API access to their tracks, http://stackoverflow.com/a/36529330
				if (((HttpWebResponse)x.Response).StatusCode == HttpStatusCode.Forbidden) {
					msg += "This track cannot be embedded.";
				}

				log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));

			} catch (JsonSerializationException x) {
				var msg = string.Format("Unable to load SoundCloud URL '{0}'.", url);
				log.Warn(x, msg);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));
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

			return VideoUrlParseResult.CreateOk(url, PVService.SoundCloud, id.ToString(), VideoTitleParseResult.CreateSuccess(title, author, thumbUrl, length, uploadDate: uploadDate));

		}

		public override VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			var soundCloudUrl = linkMatchers[0].GetId(url);

			return ParseBySoundCloudUrl(soundCloudUrl);

		}

	}

	/// <summary>
	/// Composite SoundCloud ID. Contains both the track Id and the relative URL (for direct links).
	/// </summary>
	public class SoundCloudId {

		public SoundCloudId(string trackId, string soundCloudUrl) {

			ParamIs.NotNullOrEmpty(() => trackId);
			ParamIs.NotNullOrEmpty(() => soundCloudUrl);

			TrackId = trackId;
			SoundCloudUrl = soundCloudUrl;

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
