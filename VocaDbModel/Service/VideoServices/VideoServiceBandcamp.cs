using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NLog;
using NYoutubeDL;
using NYoutubeDL.Models;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Web;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {
	public class VideoServiceBandcamp : VideoService {

		public static readonly RegexLinkMatcher[] Matchers =
		{
			new RegexLinkMatcher(".bandcamp.com/track/{0}", @".bandcamp.com/track/([\w\-]+)")
		};

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		private string GetErrorString(DownloadInfo info) => info != null ? string.Join(", ", info.Warnings.Concat(info.Errors)) : string.Empty;

		private string GetPath(string path) {
			// TODO: inject this.
			return path.Contains("~") ? GlobalServerPathMapper.ServerPathMapper.MapPath(path) : path;
		}

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			var youtubeDl = new YoutubeDL {
				RetrieveAllInfo = true,
				YoutubeDlPath = GetPath(AppConfig.YoutubeDLPath),
				PythonPath = GetPath(AppConfig.PythonPath)
			};

			DownloadInfo result;
			try {
				result = await youtubeDl.GetDownloadInfoAsync(url);
			} catch (TaskCanceledException) {
				var warnings = GetErrorString(youtubeDl.Info);
				_log.Error("Timeout. Error list: {0}", warnings);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Timeout");
			}

			if (result == null) {
				var warnings = GetErrorString(youtubeDl.Info);
				_log.Error("Result from parser is empty. Error list: {0}", warnings);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Result from parser is empty");
			}

			if (!(result is VideoDownloadInfo info)) {
				var warnings = GetErrorString(youtubeDl.Info);
				_log.Error("Unexpected result from parser. Error list: {0}. Result type is {1}. Title is {2}", warnings, result.GetType().Name, result.Title);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Unexpected result from parser.");
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
