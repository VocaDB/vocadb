import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';

export interface ArtistForArtistContract {
	parent: ArtistContract;

	// Link ID
	id?: number;

	linkType?: string;
}
