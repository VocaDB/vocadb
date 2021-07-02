import UserWithPermissionsContract from '@DataContracts/User/UserWithPermissionsContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';

export default interface GlobalValues {
	lockdownMessage?: string;

	/** URL of the site path, for example "/" */
	baseAddress: string;
	/** Whether the user is logged in. */
	isLoggedIn: boolean;
	loggedUserId: number;
	loggedUser?: UserWithPermissionsContract;
	languagePreference: ContentLanguagePreference;
	culture: string;
	/** UI language code, for example "en" */
	uiCulture: string;
}
