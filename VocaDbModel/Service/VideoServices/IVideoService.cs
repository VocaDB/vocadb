using System.Collections.Generic;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoService {

		IEnumerable<string> GetUserProfileUrls(string authorId);

		bool IsValidFor(string url);

		VideoUrlParseResult ParseByUrl(string url, bool getTitle);

	}

}
