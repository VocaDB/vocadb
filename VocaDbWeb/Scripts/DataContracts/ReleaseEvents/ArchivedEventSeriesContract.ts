import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { EventCategory } from '@/Models/Events/EventCategory';

export interface ArchivedEventSeriesContract {
	aliases?: string[];
	category: EventCategory;
	description: string;
	id: number;
	mainPictureMime?: string;
	names?: LocalizedStringContract[];
	translatedName: ArchivedTranslatedStringContract;
	webLinks?: ArchivedWebLinkContract[];
}
