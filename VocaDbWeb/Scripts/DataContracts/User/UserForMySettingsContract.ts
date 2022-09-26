import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { UserKnownLanguageContract } from '@/DataContracts/User/UserKnownLanguageContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { UserEmailOptions } from '@/Models/Users/UserEmailOptions';

// Corresponds to the UserForMySettingsContract record class in C#.
export interface UserForMySettingsContract {
	aboutMe: string;
	anonymousActivity: boolean;
	canChangeName: boolean;
	culture: string;
	defaultLanguageSelection: ContentLanguagePreference;
	email: string;
	emailOptions: UserEmailOptions;
	emailVerified: boolean;
	hashedAccessKey: string;
	hasPassword: boolean;
	hasTwitterToken: boolean;
	id: number;
	knownLanguages: UserKnownLanguageContract[];
	language: string;
	location: string;
	mainPicture?: EntryThumbContract;
	name: string;
	preferredVideoService: string /* TODO: enum */;
	publicAlbumCollection: boolean;
	publicRatings: boolean;
	showChatbox: boolean;
	stylesheet?: string;
	twitterName: string;
	unreadNotificationsToKeep: number;
	webLinks: WebLinkContract[];
}
