using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoService {

		bool IsValidFor(string url);

		VideoUrlParseResult ParseByUrl(string url, bool getTitle);

	}

}
