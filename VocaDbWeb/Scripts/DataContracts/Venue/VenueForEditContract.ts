
module vdb.dataContracts {

	export interface VenueForEditContract {

		coordinates: OptionalGeoPointContract;

		defaultNameLanguage: string;

		id: number;

		names?: globalization.LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

}

