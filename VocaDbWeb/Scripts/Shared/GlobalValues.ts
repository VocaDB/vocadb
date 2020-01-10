
module vdb.values {

    // URL of the site path, for example "/"
    export var baseAddress: string;

    // URL including the scheme and site path, for example "http://vocadb.net/"
	export var hostAddress: string;

	// Whether the user is logged in.
	export var isLoggedIn: boolean;

	export var languagePreference: number;

	// UI language code, for example "en"
	export var uiLanguage: string;

}