import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { ArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArtistForEventContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';

// Corresponds to the ReleaseEventForEditForApiContract record class in C#.
export interface ReleaseEventForEditContract {
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
