import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';

export interface ArtistForEventContract {
	artist?: ArtistContract;
	effectiveRoles?: string;
	id?: number;
	name?: string;
	roles: string;
}
