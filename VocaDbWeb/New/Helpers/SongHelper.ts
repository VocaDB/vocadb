import { ContentFocus } from '@/types/Models/ContentFocus';
import { SongType } from '@/types/Models/Songs/SongType';

export class SongHelper {
	static originalVersionTypes = [
		SongType.Unspecified,
		SongType.Original,
		SongType.Remaster,
		SongType.Remix,
		SongType.Cover,
		SongType.Mashup,
		SongType.DramaPV,
		SongType.Other,
	];

	// Checks whether a song type is to be considered animation where animators are considered as the main role
	static getContentFocus(songType: SongType): ContentFocus {
		switch (songType) {
			case SongType.DramaPV:
			case SongType.MusicPV:
				return ContentFocus.Video;

			case SongType.Illustration:
				return ContentFocus.Illustration;

			default:
				return ContentFocus.Music;
		}
	}

	// Checks whether a song type is to be considered instrumental where the song is allowed to have no vocalists
	static isInstrumental(songType: SongType): boolean {
		return songType === SongType.Instrumental || songType === SongType.DramaPV;
	}
}
