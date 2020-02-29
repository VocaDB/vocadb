using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NLog;
using PiaproClient;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServicePiapro : VideoService {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public VideoServicePiapro(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		private VideoUrlParseResult Parse(PostQueryResult result, string url) {

			if (result.PostType != PostType.Audio) {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException("Content type indicates this isn't an audio post"));
			}

			var piaproMetadata = new PVExtendedMetadata(new PiaproMetadata {
				Timestamp = result.UploadTimestamp
			});

			return VideoUrlParseResult.CreateOk(url, PVService.Piapro, result.Id,
				VideoTitleParseResult.CreateSuccess(result.Title, result.Author, result.AuthorId, result.ArtworkUrl, result.LengthSeconds, uploadDate: result.Date, extendedMetadata: piaproMetadata));

		}

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			PostQueryResult result;
			var client = new PiaproClient.PiaproClient { RequestTimeout = TimeSpan.FromMilliseconds(3900) /* Value chosen after careful consideration */ };
			try {
				result = await client.ParseByUrlAsync(url);
			} catch (PiaproException x) {
				log.Warn(x, "Unable to load Piapro URL {0}", url);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(x.Message, x));
			}

			return Parse(result, url);

		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId) {
			return new[] {
				string.Format("http://piapro.jp/{0}", authorId),
				string.Format("https://piapro.jp/{0}", authorId),
			};
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PiaproMetadata {

		[DataMember]
		public string Timestamp { get; set; }
	}

}
