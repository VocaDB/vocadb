using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices;

public class VideoServiceYoutube : VideoService
{
	public VideoServiceYoutube(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers)
		: base(service, parser, linkMatchers) { }

	public override string GetThumbUrlById(string id)
	{
		// Use `i1.ytimg.com` instead of `img.youtube.com`.
		// See https://stackoverflow.com/a/64632873.
		return $"https://i1.ytimg.com/vi/{id}/default.jpg";
	}

	public override string GetMaxSizeThumbUrlById(string id)
	{
		// Use `i1.ytimg.com` instead of `img.youtube.com`.
		// See https://stackoverflow.com/a/64632873.
		return $"https://i1.ytimg.com/vi/{id}/hqdefault.jpg";
	}

	public override string GetUrlById(string id, PVExtendedMetadata _)
	{
		var matcher = _linkMatchers.First();
		return $"https://{matcher.MakeLinkFromId(id)}";
	}

	public override IEnumerable<string> GetUserProfileUrls(string authorId)
	{
		return Enumerable.Repeat("https://www.youtube.com/channel/" + authorId, 1);
	}
}
