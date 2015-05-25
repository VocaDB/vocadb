using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
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

			switch (entry.EntryType) {
				case EntryType.Album: {
					var album = entry as Album;
					if (album != null && album.CoverPictureData != null)
						return album.CoverPictureMime;								
				}
				break;

				case EntryType.Artist: {
					var artist = entry as Artist;
					if (artist != null && artist.Picture != null)
						return artist.PictureMime;				
					
				}
				break;

				case EntryType.SongList: {
					var songList = entry as SongList;
					if (songList != null && songList.Thumb != null)
						return songList.Thumb.Mime;									
				}
				break;

				case EntryType.Tag: {
					var tag = entry as Tag;
					if (tag != null && tag.Thumb != null)
						return tag.Thumb.Mime;									
				}
				break;

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
