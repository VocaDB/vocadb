import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { TagRepository } from '@/Repositories/TagRepository';
import { HttpClient } from '@/Shared/HttpClient';

export class FakeTagRepository extends TagRepository {
	constructor() {
		super(new HttpClient(), '');

		this.getEntryTypeTag = ({
			entryType,
			subType,
			lang,
		}: {
			entryType: EntryType;
			subType: string;
			lang: ContentLanguagePreference;
		}): Promise<TagApiContract> => {
			return Promise.resolve<TagApiContract>(null!);
		};
	}
}
