import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';

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
