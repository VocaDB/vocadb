using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.TagFormatting {

	public class AlbumSongFormatter : SongCsvFileFormatter<SongInAlbum> {

		public static readonly string[] TagFormatStrings = {
			"%title%;%title%%featvocalists%;%producers%;%album%;%discnumber%;%track%",
			"%title% feat. %vocalists%;%producers%;%album%;%discnumber%;%track%",
			"%title%;%producers%;%vocalists%;%album%;%discnumber%;%track%",
			"%title%;%artists%;%album%;%discnumber%;%track%",
		};		

		private string GetAlbumMainProducersStr(Album album, ContentLanguagePreference languagePreference) {
			var focus = AlbumHelper.GetContentFocus(album.DiscType);
			return ArtistHelper.GetArtistString(ArtistHelper.GetProducers(album.Artists.Where(a => !a.IsSupport), focus), focus)[languagePreference];
		}

		protected override string GetFieldValue(string fieldName, SongInAlbum track, ContentLanguagePreference languagePreference) {

			var album = track.Album;

			switch (fieldName) {
				// Album title
				case "album":			
					return album.Names.SortNames[languagePreference];
				case "albumid":
					return album.Id.ToString();

				// Artists for album
				case "albumartist":
				case "album artist": // foobar style
					return album.ArtistString[languagePreference];

				case "albummaincircle":
					var circle = ArtistHelper.GetMainCircle(album.Artists.ToArray(), AlbumHelper.GetContentFocus(album.DiscType));
					return (circle != null ? circle.TranslatedName[languagePreference] : GetAlbumMainProducersStr(album, languagePreference));

				case "catalognum":
					return (album.OriginalRelease != null ? album.OriginalRelease.CatNum : string.Empty);

				case "disccount":
					return (album.Songs.Any() ? album.Songs.Max(s => s.DiscNumber) : 0).ToString();

				// Disc number
				case "discnumber":		
					return track.DiscNumber.ToString();

				case "genres":
					return string.Join(", ", SongHelper.GetGenreTags(track).Select(t => t.Names.SortNames[languagePreference]));

				// Album release date
				case "releasedate":		
					return track.Album.OriginalReleaseDate.ToString();

				case "releaseyear":
					return track.Album.OriginalReleaseDate.Year.ToString();

				case "releaseevent":
					return album.OriginalReleaseEvent?.TranslatedName[languagePreference];

				// Song title
				case "title":			
					return track.Song != null ? track.Song.Names.SortNames[languagePreference] : track.Name;

				case "totaltrackcount":
					return album.Songs.Count().ToString();

				// Track number
				case "track":			
				case "tracknumber": // foobar style
					return track.TrackNumber.ToString();

				default:
					return GetFieldValue(fieldName, (ISongLink)track, languagePreference);
			}

		}

		public AlbumSongFormatter(IEntryLinkFactory entryLinkFactory) 
			: base(entryLinkFactory) {}

		public string ApplyFormat(Album album, string format, int? discNumber, ContentLanguagePreference languagePreference, bool includeHeader) {

			return ApplyFormat(album.Songs.WhereDiscNumberIs(discNumber), format, languagePreference, includeHeader);

		}

		public Dictionary<string, string>[] ApplyFormatDict(Album album, string[] fields, int? discNumber, ContentLanguagePreference languagePreference) {

			return ApplyFormatDict(album.Songs.WhereDiscNumberIs(discNumber), fields, languagePreference);

		}

	}
}
