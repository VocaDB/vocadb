using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.TestSupport {

	public class FakePVParser : IPVParser {

		public Func<string, bool, VideoUrlParseResult> ResultFunc { get; set; }

		public Dictionary<string, VideoUrlParseResult> MatchedPVs { get; set; }

		public FakePVParser() {
			MatchedPVs = new Dictionary<string, VideoUrlParseResult>(StringComparer.InvariantCultureIgnoreCase);
		}

		public VideoUrlParseResult ParseByUrl(string url, bool getTitle, IUserPermissionContext permissionContext) {

			if (!MatchedPVs.ContainsKey(url))
				return ResultFunc(url, getTitle);

			return MatchedPVs[url];

		}

	}

}
