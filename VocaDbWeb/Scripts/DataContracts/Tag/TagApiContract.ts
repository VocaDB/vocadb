import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';

export interface TagApiContract {
	additionalNames?: string;
	categoryName: string;
	defaultNameLanguage: string;
	description: string;
	id: number;
	mainPicture: EntryThumbContract;
	name: string;
	names: LocalizedStringWithIdContract[];
	parent: TagBaseContract;
	relatedTags?: TagBaseContract[];
	status: EntryStatus;
	targets: EntryType;
	translatedDescription?: EnglishTranslatedStringContract;
	urlSlug?: string;
	usageCount: number;
	version?: number;
	webLinks: WebLinkContract[];
}
