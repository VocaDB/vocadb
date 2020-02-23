
namespace vdb.dataContracts {

	export interface VenueForApiContract {

		id: number;

		name: string;

		names?: globalization.LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

}