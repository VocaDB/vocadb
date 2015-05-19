using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Helpers {

	// TODO: this class should be refactored
	public static class EntryBaseHelper {

		public static string GetArtistString(IEntryBase entry, ContentLanguagePreference languagePreference) {

			switch (entry.EntryType) {
				case EntryType.Album:
					return ((Album)entry).ArtistString[languagePreference];
				case EntryType.Song:
					return ((Song)entry).ArtistString[languagePreference];
				default:
					return null;
			}

		}

		public static EntryWithImageContract GetEntryWithImageContract(IEntryWithNames entry, ContentLanguagePreference languagePreference) {
			return new EntryWithImageContract(entry, GetMime(entry), GetSongThumbUrl(entry), languagePreference);
		}

		public static string GetMime(IEntryBase entry) {

			if (entry.EntryType == EntryType.Album) {
				var album = entry as Album;
				if (album != null && album.CoverPictureData != null)
					return album.CoverPictureMime;			
			}

			if (entry.EntryType == EntryType.Artist) {
				var artist = entry as Artist;
				if (artist != null && artist.Picture != null)
					return artist.PictureMime;				
			}

			return string.Empty;

		}

		public static string GetSongThumbUrl(IEntryBase entry) {

			if (entry == null || entry.EntryType != EntryType.Song)
				return string.Empty;

			var song = (Song)entry;
			return !string.IsNullOrEmpty(song.ThumbUrl) ? song.ThumbUrl : VideoServiceHelper.GetThumbUrl(song.PVs.PVs);

		}

	}

}
