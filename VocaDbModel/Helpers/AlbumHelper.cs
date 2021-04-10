using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Helpers
{
	public static class AlbumHelper
	{
		public static ContentFocus GetContentFocus(DiscType t) => t switch
		{
			DiscType.Artbook => ContentFocus.Illustration,
			DiscType.Video => ContentFocus.Video,
			_ => ContentFocus.Music,
		};
	}
}
