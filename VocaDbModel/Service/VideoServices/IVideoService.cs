using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices;

public interface IVideoService
{
	IEnumerable<string> GetUserProfileUrls(string authorId);

	bool IsValidFor(string url);

	bool IsValidFor(PVService service);

	Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle);
}