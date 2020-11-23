using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices
{
	public class VideoServiceYoutube : VideoService
	{
		public VideoServiceYoutube(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers)
			: base(service, parser, linkMatchers) { }

		public override string GetThumbUrlById(string id)
		{
			const string url = "https://img.youtube.com/vi/{0}/default.jpg";
			return string.Format(url, id);
		}

		public override string GetMaxSizeThumbUrlById(string id)
		{
			const string url = "https://img.youtube.com/vi/{0}/hqdefault.jpg";
			return string.Format(url, id);
		}

		public override string GetUrlById(string id, PVExtendedMetadata _)
		{
			var matcher = linkMatchers.First();
			return string.Format("https://{0}", matcher.MakeLinkFromId(id));
		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId)
		{
			return Enumerable.Repeat("https://www.youtube.com/channel/" + authorId, 1);
		}
	}
}
