import { DiscMediaType } from './AlbumDetailsForApi';

export default interface AlbumDiscPropertiesContract {
	discNumber: number;

	id: number;

	mediaType: DiscMediaType;

	name: string;
}
