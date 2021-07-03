import UserWithPermissionsContract from '@DataContracts/User/UserWithPermissionsContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';

// Corresponds to the GlobalValues record class in C#.
export default interface GlobalValues {
	lockdownMessage?: string;

	/** URL of the site path, for example "/" */
	baseAddress: string;
	languagePreference: ContentLanguagePreference;
	/** Whether the user is logged in. */
	isLoggedIn: boolean;
	loggedUserId: number;
	loggedUser?: UserWithPermissionsContract;
	culture: string;
	/** UI language code, for example "en" */
	uiCulture: string;
}
