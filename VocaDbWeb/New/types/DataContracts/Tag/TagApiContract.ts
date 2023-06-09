import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EntryType } from '@/types/Models/EntryType';

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
