import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { ArtistForEventContract } from '@/types/DataContracts/ReleaseEvents/ArtistForEventContract';
import { SongListBaseContract } from '@/types/DataContracts/SongListBaseContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EventCategory } from '@/types/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/types/Models/Globalization/ContentLanguageSelection';
import { IEntryWithIdAndName } from '@/types/Models/IEntryWithIdAndName';

// Corresponds to the ReleaseEventForEditForApiContract record class in C#.
export interface ReleaseEventForEditContract {
	artists: ArtistForEventContract[];
	category: EventCategory;
	customName: boolean;
	date?: string;
	defaultNameLanguage: ContentLanguageSelection;
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
	status: EntryStatus;
	venue?: IEntryWithIdAndName;
	venueName?: string;
	webLinks: WebLinkContract[];
}
