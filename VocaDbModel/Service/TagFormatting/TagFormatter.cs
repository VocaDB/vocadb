using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.TagFormatting {

	public class TagFormatter : SongCsvFileFormatter<SongInAlbum> {

		public static readonly string[] TagFormatStrings = new[] {
			"%title%;%title%%featvocalists%;%producers%;%album%;%discnumber%;%track%",
			"%title% feat. %vocalists%;%producers%;%album%;%discnumber%;%track%",
			"%title%;%producers%;%vocalists%;%album%;%discnumber%;%track%",
			"%title%;%artists%;%album%;%discnumber%;%track%",
		};		

		private string GetAlbumMainProducersStr(Album album, ContentLanguagePreference languagePreference) {
			bool isAnimation = AlbumHelper.IsAnimation(album.DiscType);
			return ArtistHelper.GetArtistString(ArtistHelper.GetProducers(album.Artists.Where(a => !a.IsSupport), isAnimation), isAnimation)[languagePreference];
		}

		private string GetProducerStr(SongInAlbum track, ContentLanguagePreference languagePreference) {

			if (track.Song == null)
				return string.Empty;

			return string.Join(", ", ArtistHelper.GetProducerNames(track.Song.Artists, SongHelper.IsAnimation(track.Song.SongType), languagePreference));

		}

		private string GetVocalistStr(SongInAlbum track, ContentLanguagePreference languagePreference) {

			if (track.Song == null)
				return string.Empty;

			return string.Join(", ", ArtistHelper.GetVocalistNames(track.Song.Artists, languagePreference));

		}

		protected override string GetFieldValue(string fieldName, SongInAlbum track, ContentLanguagePreference languagePreference) {

			var album = track.Album;

			switch (fieldName) {
				// Album title
				case "album":			
					return album.Names.SortNames[languagePreference];

				// Artists for album
				case "albumartist":
					return album.ArtistString[languagePreference];
				case "album artist": // foobar style
					return album.ArtistString[languagePreference];

				case "albummaincircle":
					var circle = ArtistHelper.GetMainCircle(album.Artists.ToArray(), AlbumHelper.IsAnimation(album.DiscType));
					return (circle != null ? circle.TranslatedName[languagePreference] : GetAlbumMainProducersStr(album, languagePreference));

				// Artists for song, both producers and vocalists
				case "artist":			
					return track.Song != null ? track.Song.ArtistString[languagePreference] : string.Empty;
				case "track artist": // foobar style
					return track.Song != null ? track.Song.ArtistString[languagePreference] : string.Empty;

				case "catalognum":
					return (album.OriginalRelease != null ? album.OriginalRelease.CatNum : string.Empty);

				case "disccount":
					return (album.Songs.Any() ? album.Songs.Max(s => s.DiscNumber) : 0).ToString();

				// Disc number
				case "discnumber":		
					return track.DiscNumber.ToString();

				// List of vocalists, separated by comma, with "feat." in the beginning if there are any vocalists, otherwise empty.
				case "featvocalists":	
					var vocalistStr = GetVocalistStr(track, languagePreference);
					return (vocalistStr.Any() ? " feat. " + vocalistStr : string.Empty);

				case "genres":
					return string.Join(", ", SongHelper.GetGenreTags(track).Select(t => t.NameWithSpaces));

				// List of producers
				case "producers":		
					return GetProducerStr(track, languagePreference);

				// Album release date
				case "releasedate":		
					return track.Album.OriginalReleaseDate.ToString();

				case "releaseyear":
					return track.Album.OriginalReleaseDate.Year.ToString();

				case "releaseevent":
					return album.OriginalReleaseEventName;

				// Song title
				case "title":			
					return track.Song != null ? track.Song.Names.SortNames[languagePreference] : track.Name;

				case "totaltrackcount":
					return album.Songs.Count().ToString();

				// Track number
				case "track":			
					return track.TrackNumber.ToString();
				case "tracknumber": // foobar style
					return track.TrackNumber.ToString();

				// List of vocalists, separated by comma.
				case "vocalists":		
					return GetVocalistStr(track, languagePreference);

				default:
					return string.Empty;
			}

		}

		public string ApplyFormat(Album album, string format, ContentLanguagePreference languagePreference, bool includeHeader) {

			return ApplyFormat(album.Songs, format, languagePreference, includeHeader);

		}

	}
}
