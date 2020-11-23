using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Helpers
{
	public static class AlbumHelper
	{
		public static ContentFocus GetContentFocus(DiscType t)
		{
			switch (t)
			{
				case DiscType.Artbook:
					return ContentFocus.Illustration;
				case DiscType.Video:
					return ContentFocus.Video;
				default:
					return ContentFocus.Music;
			}
		}
	}
}
