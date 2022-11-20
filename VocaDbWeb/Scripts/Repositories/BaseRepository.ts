import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { NameMatchMode } from '@/Models/NameMatchMode';

export class BaseRepository {
	protected getDate(date?: Date): string | undefined {
		return date ? date.toISOString() : undefined;
	}

	constructor(readonly baseUrl: string) {}
}

// Common parameters for entry queries (listings).
export interface CommonQueryParams {
	getTotalCount?: boolean;
	// Content language preference
	lang?: ContentLanguagePreference;
	maxResults?: number;
	nameMatchMode?: NameMatchMode;
	start?: number;
	query?: string;
}
