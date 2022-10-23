import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { FakePromise } from '@/Tests/TestSupport/FakePromise';

export class FakeArtistRepository extends ArtistRepository {
	public result: ArtistApiContract = null!;

	public constructor() {
		super(new HttpClient(), '');

		this.getOne = ({
			id,
			lang,
		}: {
			id: number;
			lang: ContentLanguagePreference;
		}): Promise<ArtistApiContract> => {
			return FakePromise.resolve(this.result);
		};
	}
}
