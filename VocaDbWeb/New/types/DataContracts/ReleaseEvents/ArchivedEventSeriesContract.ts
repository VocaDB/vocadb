import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { EventCategory } from '@/types/Models/Events/EventCategory';

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
