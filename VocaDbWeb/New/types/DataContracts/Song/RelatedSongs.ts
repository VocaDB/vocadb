import { SongApiContract } from './SongApiContract';

export interface RelatedSongs {
	artistMatches: SongApiContract[];
	likeMatches: SongApiContract[];
	tagMatches: SongApiContract[];
}
