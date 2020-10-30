using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using VocaDb.BandcampMetadataExtractor;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {
	public class VideoServiceBandcamp : VideoService {

		public static readonly RegexLinkMatcher[] Matchers =
		{
			new RegexLinkMatcher(".bandcamp.com/track/{0}", @".bandcamp.com/track/([\w\-]+)")
		};

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			var extractor = new BandcampMetadataClient();
			var info = await extractor.ExtractAsync(url);

			DateTime? date = null;
			if (DateTime.TryParseExact(info.UploadDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedDate)) {
				date = parsedDate;
			}

			var bandcampMetadata = new PVExtendedMetadata(new BandcampMetadata {
				Url = info.WebpageUrl
			});

			var meta = VideoTitleParseResult.CreateSuccess(info.Title, info.Uploader, info.UploaderId, info.Thumbnail, (int?)info.Duration, uploadDate: date, extendedMetadata: bandcampMetadata);
			return VideoUrlParseResult.CreateOk(url, PVService.Bandcamp, info.Id, meta);

		}

		public override string GetUrlById(string id, PVExtendedMetadata extendedMetadata = null) {
			var bandcampMetadata = extendedMetadata?.GetExtendedMetadata<BandcampMetadata>();
			return bandcampMetadata?.Url ?? base.GetUrlById(id, extendedMetadata);
		}

		public VideoServiceBandcamp() : base(PVService.Bandcamp, null, Matchers) {}

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class BandcampMetadata {
		[DataMember]
		public string Url { get; set; }
	}
}
