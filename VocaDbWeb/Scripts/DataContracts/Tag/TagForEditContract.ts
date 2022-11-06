import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';

// Corresponds to the TagForEditForApiContract record class in C#.
export interface TagForEditContract {
	canDelete: boolean;
	categoryName: string;
	defaultNameLanguage: string /* TODO: enum */;
	deleted: boolean;
	description: EnglishTranslatedStringContract;
	hideFromSuggestions: boolean;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	names: LocalizedStringWithIdContract[];
	parent?: TagBaseContract;
	relatedTags: TagBaseContract[];
	status: EntryStatus;
	targets: EntryType;
	updateNotes: string;
	webLinks: WebLinkContract[];
}
