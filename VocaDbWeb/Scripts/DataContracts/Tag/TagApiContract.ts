
module vdb.dataContracts {

	export interface TagApiContract {

		additionalNames?: string;

		aliasedTo: TagBaseContract;

		categoryName: string;

		defaultNameLanguage: string;

		description: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

		names: globalization.LocalizedStringWithIdContract[];

		parent: TagBaseContract;

		relatedTags?: TagBaseContract[];

		status: string;

		translatedDescription?: globalization.EnglishTranslatedStringContract;

		urlSlug?: string;

		usageCount: number;

	}

} 