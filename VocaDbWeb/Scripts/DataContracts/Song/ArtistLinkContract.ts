import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';

export interface ArtistLinkContract {
	artist: ArtistApiContract;
	categories: string /* TODO: enum */;
	effectiveRoles: string /* TODO: enum */;
	isExternal: boolean;
	isSupport: boolean;
	name: string;
}
