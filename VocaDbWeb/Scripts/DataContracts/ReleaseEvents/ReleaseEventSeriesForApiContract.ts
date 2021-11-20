import EntryThumbContract from '../EntryThumbContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

export default interface ReleaseEventSeriesForApiContract {
	additionalNames: string;

	category: string;

	description: string;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	names?: LocalizedStringWithIdContract[];

	urlSlug: string;

	webLinks: WebLinkContract[];
}
