
module vdb.helpers {

	import cls = vdb.models;
	import SongType = cls.songs.SongType;

	export class SongHelper {

		public static originalVersionTypes = [
			SongType.Unspecified, SongType.Original, SongType.Remaster, SongType.Remix, SongType.Cover, SongType.Mashup, SongType.DramaPV, SongType.Other
		];

		public static originalVersionTypesString = () => _.map(SongHelper.originalVersionTypes, s => cls.songs.SongType[s]).join(",");
		
		// Checks whether a song type is to be considered animation where animators are considered as the main role
		public static getContentFocus(songType: cls.songs.SongType) {
			return (songType === cls.songs.SongType.MusicPV || songType === cls.songs.SongType.DramaPV ? cls.ContentFocus.Video : cls.ContentFocus.Music);
		}

		// Checks whether a song type is to be considered instrumental where the song is allowed to have no vocalists
		public static isInstrumental(songType: cls.songs.SongType) {
			return songType === cls.songs.SongType.Instrumental || songType === cls.songs.SongType.DramaPV;
		}

	}

}