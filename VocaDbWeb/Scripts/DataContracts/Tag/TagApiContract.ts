
//module vdb.dataContracts {

	export interface TagApiContract {

		additionalNames?: string;

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

		targets: models.EntryType;

		translatedDescription?: globalization.EnglishTranslatedStringContract;

		urlSlug?: string;

		usageCount: number;

		webLinks: dc.WebLinkContract[];

	}

//} 