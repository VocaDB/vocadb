
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import EntryThumbContract from '../EntryThumbContract';
import EntryType from '../../Models/EntryType';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import TagBaseContract from './TagBaseContract';

//module vdb.dataContracts {

	export default interface TagApiContract {

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

		status: string;

		targets: EntryType;

		translatedDescription?: EnglishTranslatedStringContract;

		urlSlug?: string;

		usageCount: number;

		webLinks: dc.WebLinkContract[];

	}

//} 