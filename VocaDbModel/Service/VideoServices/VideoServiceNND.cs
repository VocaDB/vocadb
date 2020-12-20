#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices
{
	public class VideoServiceNND : VideoService
	{
		private static readonly Regex s_numIdRegex = new(@"(\d{6,12})");

		public VideoServiceNND(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers)
			: base(service, parser, linkMatchers) { }


		public override string GetThumbUrlById(string id)
		{
			var numId = s_numIdRegex.Match(id);

			if (!numId.Success)
				return null;

			return $"https://tn.smilevideo.jp/smile?i={numId.Value}";
		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId) => NicoHelper.GetUserProfileUrlById(authorId);
	}
}
