import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';

import EntryThumbContract from '../EntryThumbContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongListBaseContract from '../SongListBaseContract';
import WebLinkContract from '../WebLinkContract';
import ArtistForEventContract from './ArtistForEventContract';

// Corresponds to the ReleaseEventForEditForApiContract record class in C#.
export default interface ReleaseEventForEditContract {
	artists: ArtistForEventContract[];
	category: string /* TODO: enum */;
	customName: boolean;
	date?: string;
	defaultNameLanguage: string;
	deleted: boolean;
	description: string;
	endDate?: string;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	names: LocalizedStringWithIdContract[];
	pvs: PVContract[];
	series?: IEntryWithIdAndName;
	seriesNumber: number;
	seriesSuffix: string;
	songList?: SongListBaseContract;
	status: string /* TODO: enum */;
	venue?: IEntryWithIdAndName;
	venueName?: string;
	webLinks: WebLinkContract[];
}
