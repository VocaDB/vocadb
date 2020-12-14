#nullable disable

using System;
using System.Threading.Tasks;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices
{
	public class VideoServiceLocalFile : VideoService
	{
		public VideoServiceLocalFile()
			: base(PVService.LocalFile, null, new RegexLinkMatcher[0]) { }

		public override string GetIdByUrl(string url)
		{
			throw new NotSupportedException();
		}

		public override string GetThumbUrlById(string id)
		{
			if (LocalFileManager.IsImage(id))
				return VocaUriBuilder.StaticResource("/media-thumb/" + id);
			return string.Empty;
		}

		public override string GetMaxSizeThumbUrlById(string id)
		{
			return string.Empty;
		}

		public override string GetUrlById(string id, PVExtendedMetadata _)
		{
			return VocaUriBuilder.StaticResource("/media/" + id);
		}

		public override Task<VideoTitleParseResult> GetVideoTitleAsync(string id)
		{
			throw new NotSupportedException();
		}

		public override Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle)
		{
			throw new NotSupportedException();
		}

		protected override Task<VideoUrlParseResult> ParseByIdAsync(string id, string url, bool getMeta)
		{
			return ParseByUrlAsync(url, getMeta);
		}
	}
}
