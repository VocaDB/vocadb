import ArtistContract from '@/DataContracts/Artist/ArtistContract';

export default interface ArtistForEventContract {
	artist?: ArtistContract;

	effectiveRoles?: string;

	id?: number;

	name?: string;

	roles: string;
}
