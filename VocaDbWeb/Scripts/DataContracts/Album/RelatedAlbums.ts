import { AlbumForApiContract } from './AlbumForApiContract';

export interface RelatedAlbums {
	artistMatches: AlbumForApiContract[];
	likeMatches: AlbumForApiContract[];
	tagMatches: AlbumForApiContract[];
}
