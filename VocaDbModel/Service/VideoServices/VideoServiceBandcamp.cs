using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FluentNHibernate.Utils;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Utils;
using VocaDb.VideoInfoExtractors;

namespace VocaDb.Model.Service.VideoServices {
	public class VideoServiceBandcamp : VideoService {

		public static readonly RegexLinkMatcher[] Matchers =
		{
			new RegexLinkMatcher(".bandcamp.com/track/{0}", @".bandcamp.com/track/([\w\-]+)")
		};

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		//private string GetErrorString(DownloadInfo info) => info != null ? string.Join(", ", info.Warnings.Concat(info.Errors)) : string.Empty;

		private string GetPath(string path) {
			// TODO: inject this.
			return path.Contains("~") ? HttpContext.Current.Server.MapPath(path) : path;
		}

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			/*DownloadInfo result;
			try {
				var task = youtubeDl.GetDownloadInfoAsync(url);
				result = await TimeoutAfter(task, 10000);
			} catch (TaskCanceledException) {
				var warnings = GetErrorString(youtubeDl.Info);
				_log.Error("Timeout. Error list: {0}", warnings);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Timeout");
			} catch (TimeoutException) {
				youtubeDl.CancelDownload();
				_log.Error("Timeout");
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
			}*/
			var extractor = new BandcampInfoExtractor();
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

		/// <summary>
		/// Add timeout to task.
		/// Code from https://devblogs.microsoft.com/pfxteam/crafting-a-task-timeoutafter-method/
		/// </summary>
		/// <exception cref="TimeoutException">If task timed out.</exception>
		private static async Task<T> TimeoutAfter<T>(Task<T> task, int millisecondsTimeout) {
			if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
				return await task;
			else
				throw new TimeoutException();
		}

		private void StandardErrorEvent(object sender, string e) => _log.Debug($"Bandcamp:{e}");

		private void StandardOutputEvent(object sender, string e) => _log.Error($"Bandcamp: {e}");

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
