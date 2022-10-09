import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArchivedPVContract } from '@/DataContracts/PVs/ArchivedPVContract';
import { ArchivedArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArchivedArtistForEventContract';
import { EventCategory } from '@/Models/Events/EventCategory';

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
