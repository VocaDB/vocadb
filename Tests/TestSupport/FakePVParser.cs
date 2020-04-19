using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.TestSupport {

	public class FakePVParser : IPVParser {

		public Func<VocaDbUrl, bool, VideoUrlParseResult> ResultFunc { get; set; }

		public Dictionary<VocaDbUrl, VideoUrlParseResult> MatchedPVs { get; set; }

		public FakePVParser() {
			MatchedPVs = new Dictionary<VocaDbUrl, VideoUrlParseResult>();
		}

		public void AddResult(VideoUrlParseResult result) {
			MatchedPVs.Add(result.Url, result);
		}

		private VideoUrlParseResult ParseByUrl(VocaDbUrl url, bool getTitle, IUserPermissionContext permissionContext) {

			if (!MatchedPVs.ContainsKey(url)) {
				return ResultFunc != null ? ResultFunc(url, getTitle) : VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939", VideoTitleParseResult.Empty);
			}

			return MatchedPVs[url];

		}

		public Task<VideoUrlParseResult> ParseByUrlAsync(VocaDbUrl url, bool getTitle, IUserPermissionContext permissionContext) {
			return Task.FromResult(ParseByUrl(url, getTitle, permissionContext));
		}

		public Task<VideoUrlParseResult[]> ParseByUrlsAsync(IEnumerable<VocaDbUrl> urls, bool getTitle, IUserPermissionContext permissionContext) {
			return Task.FromResult(urls.Select(url => ParseByUrl(url, getTitle, permissionContext)).ToArray());
		}
	}

}
