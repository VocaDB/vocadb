
module vdb.repositories {

	import cls = vdb.models;

	export class BaseRepository {

		// todo: protected
		public languagePreferenceStr: string;

		constructor(public baseUrl: string, languagePreference = cls.globalization.ContentLanguagePreference.Default) {
			this.languagePreferenceStr = cls.globalization.ContentLanguagePreference[languagePreference];
		}

	}

	// Common parameters for entry queries (listings).
	export interface CommonQueryParams {

		getTotalCount?: boolean;

		// Content language preference
		lang?: vdb.models.globalization.ContentLanguagePreference;

		maxResults?: number;

		nameMatchMode?: cls.NameMatchMode;

		start?: number;

		query?: string;

	}

}