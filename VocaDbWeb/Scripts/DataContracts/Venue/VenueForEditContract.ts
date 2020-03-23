
module vdb.dataContracts {

	export interface VenueForEditContract {

		address: string;

		coordinates: OptionalGeoPointContract;

		defaultNameLanguage: string;

		id: number;

		names?: globalization.LocalizedStringWithIdContract[];

		addressCountryCode: string;

		webLinks: WebLinkContract[];

	}

}

