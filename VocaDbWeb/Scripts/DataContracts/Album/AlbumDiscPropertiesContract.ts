import { DiscMediaType } from '@/DataContracts/Album/AlbumDetailsForApi';

export interface AlbumDiscPropertiesContract {
	discNumber?: number;

	id: number;

	mediaType: DiscMediaType;

	name: string;
}
