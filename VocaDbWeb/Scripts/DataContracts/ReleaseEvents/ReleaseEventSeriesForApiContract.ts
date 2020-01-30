
namespace vdb.dataContracts {

	export interface ReleaseEventSeriesForApiContract {

		category: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		names?: globalization.LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

}