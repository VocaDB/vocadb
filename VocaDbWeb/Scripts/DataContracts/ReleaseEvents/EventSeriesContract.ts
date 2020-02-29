
namespace vdb.dataContracts {

	export interface EventSeriesContract extends models.IEntryWithIdAndName {

		category: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		names?: globalization.LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

}