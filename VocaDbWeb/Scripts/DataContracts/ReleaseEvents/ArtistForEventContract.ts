import ArtistContract from '../Artist/ArtistContract';

export default interface ArtistForEventContract {
	artist?: ArtistContract;

	id?: number;

	name?: string;

	roles: string;
}
