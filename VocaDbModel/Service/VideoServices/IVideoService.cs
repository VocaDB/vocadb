using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoService {

		IEnumerable<string> GetUserProfileUrls(string authorId);

		bool IsValidFor(VocaDbUrl url);

		Task<VideoUrlParseResult> ParseByUrlAsync(VocaDbUrl url, bool getTitle);

	}

}
