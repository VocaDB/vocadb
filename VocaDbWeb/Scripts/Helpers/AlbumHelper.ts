import { AlbumType } from '@/Models/Albums/AlbumType';
import { ContentFocus } from '@/Models/ContentFocus';

export class AlbumHelper {
	public static getContentFocus = (albumType: AlbumType): ContentFocus => {
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
