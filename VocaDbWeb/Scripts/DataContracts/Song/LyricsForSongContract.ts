import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';

export interface LyricsForSongContract {
	cultureCode?: string;
	id?: number;
	language?: ContentLanguageSelection;
	source: string;
	translationType: string;
	url: string;
	value?: string;
}
