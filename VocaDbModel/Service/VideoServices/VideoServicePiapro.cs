using System.Runtime.Serialization;
using PiaproClient;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServicePiapro : VideoService {

		public VideoServicePiapro(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			PostQueryResult result;
			try {
				result = new PiaproClient.PiaproClient().ParseByUrl(url);
			} catch (PiaproException x) {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(x.Message, x));
			}

			if (result.PostType != PostType.Audio) {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException("Content type indicates this isn't an audio post"));				
			}

			var piaproMetadata = new PVExtendedMetadata(new PiaproMetadata {
				Timestamp = result.UploadTimestamp
			});

			return VideoUrlParseResult.CreateOk(url, PVService.Piapro, result.Id,
				VideoTitleParseResult.CreateSuccess(result.Title, result.Author, string.Empty, string.Empty, result.LengthSeconds, uploadDate: result.Date, extendedMetadata: piaproMetadata));

		}

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PiaproMetadata {

		[DataMember]
		public string Timestamp { get; set; }
	}

}
