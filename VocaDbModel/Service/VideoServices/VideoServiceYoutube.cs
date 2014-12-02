using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceYoutube : VideoService {

		public VideoServiceYoutube(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override string GetThumbUrlById(string id) {

			const string url = "http://img.youtube.com/vi/{0}/default.jpg";
			return string.Format(url, id);

		}

		public override string GetMaxSizeThumbUrlById(string id) {
		
			const string url = "http://img.youtube.com/vi/{0}/hqdefault.jpg";
			return string.Format(url, id);
	
		}

	}
}
