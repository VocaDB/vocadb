import { ContentLanguageSelection } from '@/types/Models/Globalization/ContentLanguageSelection';

export interface LocalizedStringContract {
	language: ContentLanguageSelection;
	value: string;
}
