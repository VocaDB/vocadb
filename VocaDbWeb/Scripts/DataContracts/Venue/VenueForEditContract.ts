
module vdb.dataContracts {

	export interface VenueForEditContract {

		address: string;

		coordinates: OptionalGeoPointContract;

		defaultNameLanguage: string;

		id: number;

		names?: globalization.LocalizedStringWithIdContract[];

		regionCode: string;

		webLinks: WebLinkContract[];

	}

}

