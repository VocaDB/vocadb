using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;
using NLog;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceSoundCloud : VideoService {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private int? GetLength(string lengthStr) {

			int val;
			if (int.TryParse(lengthStr, out val))
				return val/1000;
			else
				return null;

		}

		public VideoServiceSoundCloud(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override string GetUrlById(string id) {

			var compositeId = new SoundCloudId(id);
			var matcher = linkMatchers.First();
			return "http://" + matcher.MakeLinkFromId(compositeId.SoundCloudUrl);

		}

		public VideoUrlParseResult ParseBySoundCloudUrl(string url) {

			var apiUrl = string.Format("http://api.soundcloud.com/resolve?url=http://soundcloud.com/{0}&client_id=YOUR_CLIENT_ID", url);

			var request = WebRequest.Create(apiUrl);
			request.Timeout = 10000;
			XDocument doc;

			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {
					doc = XDocument.Load(stream);
				}
			} catch (WebException x) {
				log.WarnException("Unable to load SoundCloud URL " + url, x);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException("Unable to load SoundCloud URL: " + x.Message, x));
			}

			var trackIdElem = doc.XPathSelectElement("//track/id");
			var titleElem = doc.XPathSelectElement("//track/title");

			if (trackIdElem == null || titleElem == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Unable to load SoundCloud URL: Invalid response.");

			var trackId = trackIdElem.Value;
			var title = titleElem.Value;
			var author = XmlHelper.GetNodeTextOrEmpty(doc, "//track/user/username");
			var length = GetLength(XmlHelper.GetNodeTextOrEmpty(doc, "//track/duration"));

			var thumbUrl = XmlHelper.GetNodeTextOrEmpty(doc, "//track/artwork-url");

			// Substitute song thumbnail with user avatar, if no actual thumbnail is provided. This is what the SoundCloud site does as well.
			if (string.IsNullOrEmpty(thumbUrl)) {
				thumbUrl = XmlHelper.GetNodeTextOrEmpty(doc, "//track/user/avatar-url");				
			}

			var id = new SoundCloudId(trackId, url);

			return VideoUrlParseResult.CreateOk(url, PVService.SoundCloud, id.ToString(), VideoTitleParseResult.CreateSuccess(title, author, thumbUrl, length));

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
