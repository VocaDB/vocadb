import ArtistContract from '@/DataContracts/Artist/ArtistContract';

export default interface ArtistForArtistContract {
	parent: ArtistContract;

	// Link ID
	id?: number;

	linkType?: string;
}
