
module vdb.repositories {

	import cls = vdb.models;

	export class BaseRepository {

		// todo: protected
		public languagePreferenceStr: string;

		constructor(public baseUrl: string, languagePreference = cls.globalization.ContentLanguagePreference.Default) {
			this.languagePreferenceStr = cls.globalization.ContentLanguagePreference[languagePreference];
		}

	}

}