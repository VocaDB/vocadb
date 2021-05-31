import ArtistContract from './ArtistContract';

export default interface ArtistForArtistContract {
	parent: ArtistContract;

	// Link ID
	id?: number;

	linkType?: string;
}
