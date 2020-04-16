using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceYoutube : VideoService {

		public VideoServiceYoutube(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override VocaDbUrl GetThumbUrlById(string id) {

			const string url = "https://img.youtube.com/vi/{0}/default.jpg";
			return VocaDbUrl.External(string.Format(url, id));

		}

		public override VocaDbUrl GetMaxSizeThumbUrlById(string id) {
		
			const string url = "https://img.youtube.com/vi/{0}/hqdefault.jpg";
			return VocaDbUrl.External(string.Format(url, id));
	
		}

		public override VocaDbUrl GetUrlById(string id, PVExtendedMetadata _) {
			var matcher = linkMatchers.First();
			return matcher.MakeLinkFromId(id).EnsureScheme();
		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId) {
			return Enumerable.Repeat("https://www.youtube.com/channel/" + authorId, 1);
		}

	}
}
