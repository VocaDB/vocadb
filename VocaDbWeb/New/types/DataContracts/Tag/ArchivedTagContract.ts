import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { TagTargetTypes } from '@/types/Models/Tags/TagTargetTypes';

export interface ArchivedTagContract {
	categoryName: string;
	description?: string;
	descriptionEng?: string;
	hideFromSuggestions: boolean;
	id: number;
	names: LocalizedStringContract[];
	parent?: ObjectRefContract;
	relatedTags?: ObjectRefContract[];
	targets: TagTargetTypes;
	thumbMime?: string;
	translatedName: ArchivedTranslatedStringContract;
	webLinks?: ArchivedWebLinkContract[];
}
