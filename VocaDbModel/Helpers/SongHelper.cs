using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Helpers {

	public static class SongHelper {

		public static Tag[] GetGenreTags(SongInAlbum songInAlbum) {

			Tag[] genres;

			if (songInAlbum.Song != null) {

				genres = songInAlbum.Song.Tags.TagsByVotes.Where(t => t.CategoryName == TagCommonCategoryNames.Genres).ToArray();

				if (genres.Any())
					return genres.ToArray();
				
			}

			genres = songInAlbum.Album.Tags.TagsByVotes.Where(t => t.CategoryName == TagCommonCategoryNames.Genres).ToArray();
			return genres.ToArray();

		}

		public static ContentFocus GetContentFocus(SongType songType) {
			return (songType == SongType.DramaPV || songType == SongType.MusicPV) ? ContentFocus.Video : ContentFocus.Music;
		}

	}
}
