import EntryThumbContract from '@/DataContracts/EntryThumbContract';
import LocalizedStringWithIdContract from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '@/DataContracts/WebLinkContract';

export default interface ReleaseEventSeriesForApiContract {
	additionalNames: string;

	category: string;

	description: string;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	names?: LocalizedStringWithIdContract[];

	status?: string;

	urlSlug: string;

	version?: number;

	webLinks: WebLinkContract[];
}
