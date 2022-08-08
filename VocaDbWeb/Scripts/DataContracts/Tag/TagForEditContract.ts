import EntryType from '@/Models/EntryType';

import EntryThumbContract from '../EntryThumbContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';
import TagBaseContract from './TagBaseContract';

// Corresponds to the TagForEditForApiContract record class in C#.
export default interface TagForEditContract {
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
	status: string /* TODO: enum */;
	targets: EntryType;
	updateNotes: string;
	webLinks: WebLinkContract[];
}
