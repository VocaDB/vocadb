import EntryThumbContract from "../EntryThumbContract";
import LocalizedStringWithIdContract from "../Globalization/LocalizedStringWithIdContract";
import WebLinkContract from "../WebLinkContract";

//namespace vdb.dataContracts {

	export default interface ReleaseEventSeriesForApiContract {

		category: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		names?: LocalizedStringWithIdContract[];

		webLinks: WebLinkContract[];

	}

//}