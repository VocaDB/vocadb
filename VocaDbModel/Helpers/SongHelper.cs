#nullable disable

using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Helpers
{
	public static class SongHelper
	{
		public static Tag[] GetGenreTags(SongInAlbum songInAlbum)
		{
			Tag[] genres;

			if (songInAlbum.Song != null)
			{
				genres = songInAlbum.Song.Tags.TagsByVotes.Where(t => t.CategoryName == TagCommonCategoryNames.Genres).ToArray();

				if (genres.Any())
					return genres.ToArray();
			}

			genres = songInAlbum.Album.Tags.TagsByVotes.Where(t => t.CategoryName == TagCommonCategoryNames.Genres).ToArray();
			return genres.ToArray();
		}

		public static ContentFocus GetContentFocus(SongType songType)
		{
			switch (songType)
			{
				case SongType.DramaPV:
				case SongType.MusicPV:
					return ContentFocus.Video;
				case SongType.Illustration:
					return ContentFocus.Illustration;
				default:
					return ContentFocus.Music;
			}
		}
	}
}
