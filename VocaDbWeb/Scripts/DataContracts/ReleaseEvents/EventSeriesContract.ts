import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';

import EntryThumbContract from '../EntryThumbContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

// Matches ReleaseEventForApiContract
export default interface EventSeriesContract extends IEntryWithIdAndName {
	additionalNames?: string;

	category: string;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	names?: LocalizedStringWithIdContract[];

	urlSlug?: string;

	webLinks: WebLinkContract[];
}
