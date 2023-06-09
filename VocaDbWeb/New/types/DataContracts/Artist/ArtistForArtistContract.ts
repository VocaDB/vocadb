import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';

export interface ArtistForArtistContract {
	parent: ArtistContract;

	// Link ID
	id?: number;

	linkType?: string;
}
