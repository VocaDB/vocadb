import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistRepository from '@Repositories/ArtistRepository';
import HttpClient from '@Shared/HttpClient';

import FakePromise from './FakePromise';

export default class FakeArtistRepository extends ArtistRepository {
	result: ArtistContract = null!;

	constructor() {
		super(new HttpClient());

		this.getOne = (id, lang): Promise<ArtistContract> => {
			return FakePromise.resolve(this.result);
		};
	}
}
