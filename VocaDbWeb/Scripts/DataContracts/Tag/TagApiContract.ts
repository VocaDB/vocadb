
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

		status: string;

		urlSlug?: string;

	}

} 