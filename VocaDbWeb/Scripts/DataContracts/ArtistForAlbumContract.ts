import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';

export interface ArtistForAlbumContract {
	artist?: ArtistContract;
	categories?: string /* TODO: enum */;
	effectiveRoles?: string /* TODO: enum */;
	id?: number;
	isCustomName?: boolean;
	isSupport?: boolean;
	name?: string;
	roles: string;
}
