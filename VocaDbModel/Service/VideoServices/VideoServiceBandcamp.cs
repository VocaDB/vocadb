using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FluentNHibernate.Utils;
using NYoutubeDL;
using NYoutubeDL.Models;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {
	public class VideoServiceBandcamp : VideoService {

		public static readonly RegexLinkMatcher[] Matchers =
		{
			new RegexLinkMatcher(".bandcamp.com/track/{0}", @".bandcamp.com/track/([\w\-]+)")
		};

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			var youtubeDl = new YoutubeDL { RetrieveAllInfo = true };
			// TODO: inject this.
			youtubeDl.YoutubeDlPath = HttpContext.Current.Server.MapPath(AppConfig.YoutubeDLPath);

			DownloadInfo result = null;
			try {
				result = await youtubeDl.GetDownloadInfoAsync(url);
			} catch (TaskCanceledException) {
				var warnings = string.Join("\n", youtubeDl.Info.Warnings.Concat(youtubeDl.Info.Errors));
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Timeout: " + warnings);
			}


			if (result == null) {
				var warnings = youtubeDl.Info != null ? string.Join("\n", youtubeDl.Info.Warnings.Concat(youtubeDl.Info.Errors)) : string.Empty;
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Result is empty: " + warnings);
			}

			if (!(result is VideoDownloadInfo info)) {
				var warnings = string.Join("\n", result.Warnings.Concat(youtubeDl.Info.Errors));
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Unable to retrieve video information. Error list: " + warnings + ". Result type is " + result.GetType().Name + ". Title is " + result.Title);
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
