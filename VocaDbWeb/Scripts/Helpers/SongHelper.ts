
import ContentFocus from '../Models/ContentFocus';
import SongType from '../Models/Songs/SongType';

//module vdb.helpers {

	export default class SongHelper {

		public static originalVersionTypes = [
			SongType.Unspecified, SongType.Original, SongType.Remaster, SongType.Remix, SongType.Cover, SongType.Mashup, SongType.DramaPV, SongType.Other
		];

		public static originalVersionTypesString = () => _.map(SongHelper.originalVersionTypes, s => SongType[s]).join(",");
		
		// Checks whether a song type is to be considered animation where animators are considered as the main role
		public static getContentFocus(songType: SongType) {
			return (songType === SongType.MusicPV || songType === SongType.DramaPV ? ContentFocus.Video : ContentFocus.Music);
		}

		// Checks whether a song type is to be considered instrumental where the song is allowed to have no vocalists
		public static isInstrumental(songType: SongType) {
			return songType === SongType.Instrumental || songType === SongType.DramaPV;
		}

	}

//}