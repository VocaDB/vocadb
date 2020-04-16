using System;
using System.Threading.Tasks;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceLocalFile : VideoService {

		public VideoServiceLocalFile() 
			: base(PVService.LocalFile, null, new RegexLinkMatcher[0]) { }

		public override string GetIdByUrl(VocaDbUrl url) {
			throw new NotSupportedException();
		}

		public override VocaDbUrl GetThumbUrlById(string id) {
			if (LocalFileManager.IsImage(id))
				return new VocaDbUrl($"/media-thumb/{id}", UrlDomain.Static, UriKind.Relative);
			return VocaDbUrl.Empty;
		}

		public override VocaDbUrl GetMaxSizeThumbUrlById(string id) => VocaDbUrl.Empty;

		public override VocaDbUrl GetUrlById(string id, PVExtendedMetadata _) {
			return new VocaDbUrl($"/media/{id}", UrlDomain.Static, UriKind.Relative);
		}

		public override Task<VideoTitleParseResult> GetVideoTitleAsync(string id) {
			throw new NotSupportedException();
		}

		public override Task<VideoUrlParseResult> ParseByUrlAsync(VocaDbUrl url, bool getTitle) {
			throw new NotSupportedException();
		}

		protected override Task<VideoUrlParseResult> ParseByIdAsync(string id, VocaDbUrl url, bool getMeta) {
			return ParseByUrlAsync(url, getMeta);
		}

	}
}
