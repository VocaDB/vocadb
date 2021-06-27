import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';

export default interface GlobalValues {
	/** URL of the site path, for example "/" */
	baseAddress: string;

	/** Whether the user is logged in. */
	isLoggedIn: boolean;

	loggedUserId: number;

	languagePreference: ContentLanguagePreference;

	culture: string;

	/** UI language code, for example "en" */
	uiCulture: string;
}
