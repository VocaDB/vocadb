export enum DiscMediaType {
	Audio = 'Audio',
	Video = 'Video',
}

export interface AlbumDiscPropertiesContract {
	discNumber?: number;

	id: number;

	mediaType: DiscMediaType;

	name: string;
}
