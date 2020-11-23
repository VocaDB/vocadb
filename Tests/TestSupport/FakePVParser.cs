using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.TestSupport
{

	public class FakePVParser : IPVParser
	{

		public Func<string, bool, VideoUrlParseResult> ResultFunc { get; set; }

		public Dictionary<string, VideoUrlParseResult> MatchedPVs { get; set; }

		public FakePVParser()
		{
			MatchedPVs = new Dictionary<string, VideoUrlParseResult>(StringComparer.InvariantCultureIgnoreCase);
		}

		private VideoUrlParseResult ParseByUrl(string url, bool getTitle, IUserPermissionContext permissionContext)
		{

			if (!MatchedPVs.ContainsKey(url))
			{
				return ResultFunc != null ? ResultFunc(url, getTitle) : VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939", VideoTitleParseResult.Empty);
			}

			return MatchedPVs[url];

		}

		public Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle, IUserPermissionContext permissionContext)
		{
			return Task.FromResult(ParseByUrl(url, getTitle, permissionContext));
		}

		public Task<VideoUrlParseResult[]> ParseByUrlsAsync(IEnumerable<string> urls, bool getTitle, IUserPermissionContext permissionContext)
		{
			return Task.FromResult(urls.Select(url => ParseByUrl(url, getTitle, permissionContext)).ToArray());
		}
	}

}
