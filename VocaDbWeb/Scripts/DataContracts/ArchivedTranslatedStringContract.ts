import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';

export interface ArchivedTranslatedStringContract {
	defaultLanguage: ContentLanguageSelection;
	english: string;
	japanese: string;
	romaji: string;
}
