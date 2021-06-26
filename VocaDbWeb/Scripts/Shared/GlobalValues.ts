module vdb.values {
	/** URL of the site path, for example "/" */
	export var baseAddress: string;

	/** Whether the user is logged in. */
	export var isLoggedIn: boolean;

	export var loggedUserId: number;

	export var languagePreference: number;

	export var culture: string;

	/** UI language code, for example "en" */
	export var uiCulture: string;
}
