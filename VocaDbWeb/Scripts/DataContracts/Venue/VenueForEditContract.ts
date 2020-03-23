
module vdb.dataContracts {

	export interface VenueForEditContract {

		address: string;

		addressCountryCode: string;

		coordinates: OptionalGeoPointContract;

		defaultNameLanguage: string;

		id: number;

		names?: globalization.LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

}

