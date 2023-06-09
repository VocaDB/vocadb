import { ContentLanguageSelection } from '@/types/Models/Globalization/ContentLanguageSelection';

export interface LyricsForSongContract {
	cultureCodes?: string[];
	id?: number;
	language?: ContentLanguageSelection;
	source: string;
	translationType: string;
	url: string;
	value?: string;
}
