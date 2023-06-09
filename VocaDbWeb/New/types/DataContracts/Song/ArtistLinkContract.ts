import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';

export interface ArtistLinkContract {
	artist: ArtistApiContract;
	categories: string /* TODO: enum */;
	effectiveRoles: string /* TODO: enum */;
	isSupport: boolean;
	name: string;
}
