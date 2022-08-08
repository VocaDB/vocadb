import EntryThumbContract from '@/DataContracts/EntryThumbContract';
import ReleaseEventContract from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import TagUsageForApiContract from '@/DataContracts/Tag/TagUsageForApiContract';
import WebLinkContract from '@/DataContracts/WebLinkContract';

// Corresponds to the ReleaseEventSeriesDetailsForApiContract record class in C#.
export default interface ReleaseEventSeriesDetailsContract {
	additionalNames: string;
	category: string /* TODO: enum */;
	deleted: boolean;
	description: string;
	events: ReleaseEventContract[];
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	status: string /* TODO: enum */;
	tags: TagUsageForApiContract[];
	webLinks: WebLinkContract[];
}
