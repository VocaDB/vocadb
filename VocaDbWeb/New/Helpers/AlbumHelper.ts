import { AlbumType } from '@/types/Models/Albums/AlbumType';
import { ContentFocus } from '@/types/Models/ContentFocus';

export class AlbumHelper {
	static getContentFocus = (albumType: AlbumType): ContentFocus => {
		switch (albumType) {
			case AlbumType.Artbook:
				return ContentFocus.Illustration;

			case AlbumType.Video:
				return ContentFocus.Video;

			default:
				return ContentFocus.Music;
		}
	};
}
