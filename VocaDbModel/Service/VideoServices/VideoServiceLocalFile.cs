using System;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceLocalFile : VideoService {

		public VideoServiceLocalFile() 
			: base(PVService.LocalFile, null, new RegexLinkMatcher[0]) { }

		public override string GetIdByUrl(string url) {
			throw new NotSupportedException();
		}

		public override string GetThumbUrlById(string id) {
			return string.Empty;
		}

		public override string GetMaxSizeThumbUrlById(string id) {
			return string.Empty;
		}

		public override string GetUrlById(string id) {
			return VocaUriBuilder.StaticResource("/media/" + id);
		}

		public override VideoTitleParseResult GetVideoTitle(string id) {
			throw new NotSupportedException();
		}

		public override VideoUrlParseResult ParseByUrl(string url, bool getTitle) {
			throw new NotSupportedException();
		}

		protected override VideoUrlParseResult ParseById(string id, string url, bool getMeta) {
			return ParseByUrl(url, getMeta);
		}

	}
}
