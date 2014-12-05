
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

		// Comma-separated list of optional fields
		fields?: string;

		// Content language preference
		lang?: string;

		nameMatchMode?: string;

	}

}