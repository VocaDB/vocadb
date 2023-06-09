import { UserKnownLanguageContract } from '@/types/DataContracts/User/UserKnownLanguageContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { ContentLanguagePreference } from '@/types/Models/Globalization/ContentLanguagePreference';
import { UserEmailOptions } from '@/types/Models/Users/UserEmailOptions';

// Corresponds to the ServerOnlyUpdateUserSettingsForApiContract record class in C#.
export interface UpdateUserSettingsContract {
	aboutMe: string;
	anonymousActivity: boolean;
	culture: string;
	defaultLanguageSelection: ContentLanguagePreference;
	email: string;
	emailOptions: UserEmailOptions;
	id: number;
	knownLanguages: UserKnownLanguageContract[];
	language: string;
	location: string;
	name: string;
	newPass: string;
	newPassAgain: string;
	oldPass: string;
	preferredVideoService: string /* TODO: enum */;
	publicAlbumCollection: boolean;
	publicRatings: boolean;
	showChatbox: boolean;
	stylesheet: string;
	unreadNotificationsToKeep: number;
	webLinks: WebLinkContract[];
}
