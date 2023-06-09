import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArchivedPVContract } from '@/types/DataContracts/PVs/ArchivedPVContract';
import { ArchivedArtistForEventContract } from '@/types/DataContracts/ReleaseEvents/ArchivedArtistForEventContract';
import { EventCategory } from '@/types/Models/Events/EventCategory';

export interface ArchivedEventContract {
	artists?: ArchivedArtistForEventContract[];
	category: EventCategory;
	date?: string;
	description: string;
	id: number;
	mainPictureMime?: string;
	name?: string;
	names?: LocalizedStringContract[];
	pvs?: ArchivedPVContract[];
	series?: ObjectRefContract;
	seriesNumber: number;
	songList?: ObjectRefContract;
	translatedName: ArchivedTranslatedStringContract;
	venue?: ObjectRefContract;
	venueName?: string;
	webLinks?: ArchivedWebLinkContract[];
}
