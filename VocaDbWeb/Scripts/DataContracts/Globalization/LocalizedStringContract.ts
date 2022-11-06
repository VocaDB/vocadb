import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';

export interface LocalizedStringContract {
	language: ContentLanguageSelection;
	value: string;
}
