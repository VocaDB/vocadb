import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { TagTargetTypes } from '@/types/Models/Tags/TagTargetTypes';

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
	targets: TagTargetTypes;
	updateNotes: string;
	webLinks: WebLinkContract[];
}
