import EntryThumbContract from '../EntryThumbContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

// Corresponds to the ReleaseEventSeriesForEditForApiContract record class in C#.
export default interface ReleaseEventSeriesForEditContract {
	category: string /* TODO: enum */;
	defaultNameLanguage: string;
	deleted: boolean;
	description: string;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	names: LocalizedStringWithIdContract[];
	status: string /* TODO: enum */;
	webLinks: WebLinkContract[];
}
