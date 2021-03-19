import EntryThumbContract from '../EntryThumbContract';
import IEntryWithIdAndName from '../../Models/IEntryWithIdAndName';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

	// Matches ReleaseEventForApiContract
	export default interface EventSeriesContract extends IEntryWithIdAndName {

		category: string;

		id: number;

		mainPicture?: EntryThumbContract;

		name: string;

		names?: LocalizedStringWithIdContract[];

		urlSlug?: string;

		webLinks: WebLinkContract[];

	}