using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NYoutubeDL;
using NYoutubeDL.Models;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {
	public class VideoServiceBandcamp : VideoService {

		public static readonly RegexLinkMatcher[] Matchers =
		{
			new RegexLinkMatcher(".bandcamp.com/track/{0}", @".bandcamp.com/track/([\w\-]+)")
		};

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			var youtubeDl = new YoutubeDL { RetrieveAllInfo = true };			
			var result = await youtubeDl.GetDownloadInfoAsync(url);

			if (!(result is VideoDownloadInfo info)) {
				var errors = string.Join(", ", result.Errors);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Unable to retrieve video information. Error list: " + errors);
			}

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
