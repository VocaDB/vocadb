
module vdb.dataContracts {

	export interface VenueForEditContract {

		defaultNameLanguage: string;

		id: number;

		names?: globalization.LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

}

