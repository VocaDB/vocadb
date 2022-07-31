import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { TagTargetTypes } from '@/Models/Tags/TagTargetTypes';

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
