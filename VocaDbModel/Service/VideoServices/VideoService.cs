using System.Linq;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoService : IVideoService {

		public static readonly VideoService Bilibili = new VideoServiceBilibili();

		public static readonly VideoService NicoNicoDouga =
			new VideoServiceNND(PVService.NicoNicoDouga, new NicoParser(), new[] {
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/([a-z]{2}\d{4,10})"),
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/(\d{6,12})"),
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nico.ms/([a-z]{2}\d{4,10})"),
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nico.ms/(\d{6,12})")
			});

		public static readonly VideoService Piapro =
			new VideoServicePiapro(PVService.Piapro, null, new[] {
				new RegexLinkMatcher("piapro.jp/content/{0}", @"piapro.jp/t/([\w\-]+)"),
				new RegexLinkMatcher("piapro.jp/content/{0}", @"piapro.jp/content/([\w\-]+)"),
			});

		public static readonly VideoService SoundCloud =
			new VideoServiceSoundCloud(PVService.SoundCloud, null, new[] {
				new RegexLinkMatcher("soundcloud.com/{0}", @"soundcloud.com/(\S+)"),
			});

		public static readonly VideoService Youtube =
			new VideoServiceYoutube(PVService.Youtube, new YoutubeParser(), new[] {
				new RegexLinkMatcher("youtu.be/{0}", @"youtu.be/(\S{11})"),
				new RegexLinkMatcher("www.youtube.com/watch?v={0}", @"youtube.com/watch?\S*v=(\S{11})"),
			});

		public static readonly VideoService Vimeo =
			new VideoService(PVService.Vimeo, new VimeoParser(), new[] {
				new RegexLinkMatcher("vimeo.com/{0}", @"vimeo.com/(\d+)"),
			});

		public static readonly VideoServiceFile File =
			new VideoServiceFile();

		protected readonly RegexLinkMatcher[] linkMatchers;
		private readonly IVideoServiceParser parser;

		protected VideoService(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) {
			Service = service;
			this.parser = parser;
			this.linkMatchers = linkMatchers;
		}

		public PVService Service { get; private set; }

		public virtual string GetIdByUrl(string url) {

			var matcher = linkMatchers.FirstOrDefault(m => m.IsMatch(url));

			if (matcher == null)
				return null;

			return matcher.GetId(url);

		}

		public virtual string GetThumbUrlById(string id) {

			return null;

		}

		public virtual string GetMaxSizeThumbUrlById(string id) {
			return GetThumbUrlById(id);
		}

		public virtual string GetUrlById(string id) {

			var matcher = linkMatchers.First();
			return string.Format("http://{0}", matcher.MakeLinkFromId(id));

		}

		public virtual VideoTitleParseResult GetVideoTitle(string id) {

			return (parser != null ? parser.GetTitle(id) : null);

		}

		/// <summary>
		/// Tests whether the user has the required permissions to add PVs for this service.
		/// </summary>
		/// <param name="permissionContext">Permission context. Can be null (when no user is logged in).</param>
		/// <returns>True if the user authorized to add PVs for this service, otherwise false.</returns>
		public virtual bool IsAuthorized(IUserPermissionContext permissionContext) {
			return true;
		}

		public virtual bool IsValidFor(string url) {

			return linkMatchers.Any(m => m.IsMatch(url));

		}

		public virtual VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			var id = GetIdByUrl(url);

			if (id == null) {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher);
			}

			return ParseById(id, url, getTitle);

		}

		protected virtual VideoUrlParseResult ParseById(string id, string url, bool getMeta) {

			var meta = (getMeta ? GetVideoTitle(id) : VideoTitleParseResult.Empty);

			//if (!meta.Success) {
			//	return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, meta.Error);
			//}

			// Note that even if meta lookup failed, we're returning Ok here, because for example NND API doesn't support all PVs.

			return VideoUrlParseResult.CreateOk(url, Service, id, meta);

		}

	}

}
