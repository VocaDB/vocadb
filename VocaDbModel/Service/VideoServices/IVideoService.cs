using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VocaDb.Model.Service.VideoServices
{
	public interface IVideoService
	{
		IEnumerable<string> GetUserProfileUrls(string authorId);

		bool IsValidFor(string url);

		Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle);
	}
}
